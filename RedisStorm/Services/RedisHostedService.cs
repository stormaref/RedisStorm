using System.Reflection;
using System.Text.Json;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisStorm.Attributes;
using RedisStorm.Exceptions;
using RedisStorm.Extensions;
using RedisStorm.Interfaces;
using RedisStorm.Registration;
using StackExchange.Redis;
using ISubscriber = RedisStorm.Interfaces.ISubscriber;

namespace RedisStorm.Services;

public class RedisHostedService : IHostedService
{
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly IServiceScopeFactory _scopeFactory;

    public RedisHostedService(IServiceScopeFactory scopeFactory)
    {
        _multiplexer = DependencyStore.Multiplexer ?? throw new ConnectionMultiplexerException();
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await GetSubscribersFromStore(cancellationToken);
        if (DependencyStore.ShouldScanAssemblyForSubscribers) await GetSubscribersFromAssembly(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task GetSubscribersFromStore(CancellationToken cancellationToken)
    {
        foreach (var channel in DependencyStore.ChannelSubscriberDictionary.Keys)
            await Subscribe(channel, DependencyStore.ChannelSubscriberDictionary[channel], cancellationToken);
    }

    private async Task Subscribe(string channelName, Type subscriberType, CancellationToken cancellationToken)
    {
        var messageType = subscriberType.GetMessageTypeOfSubscriberType();

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService(typeof(ISubscriber<>)
                .MakeGenericType(messageType));

        await _multiplexer.GetSubscriber().SubscribeAsync(channelName, async (channel, message) =>
        {
            if (!message.HasValue) return;

            var raw = (string)message!;
            var obj = GetMessage(raw, messageType);
            await (Task)subscriberType.GetMethod("Handle")!.Invoke(service, new[] { obj, cancellationToken })!;
        });
    }

    private static object GetMessage(string raw, Type messageType)
    {
        switch (DependencyStore.SubscribingSerializationType)
        {
            case SerializationType.Json:
                return JsonSerializer.Deserialize(raw, messageType)!;
            case SerializationType.MessagePack:
                var bytes = Convert.FromBase64String(raw);
                return MessagePackSerializer.Deserialize(messageType, bytes)!;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task GetSubscribersFromAssembly(CancellationToken cancellationToken)
    {
        var types = DependencyStore.Assembly
            .GetTypes()
            .Where(t => t.IsClass &&
                        t.GetInterfaces().Contains(typeof(ISubscriber)))
            .ToList();

        foreach (var subscriberType in types)
        {
            var attribute = subscriberType.GetCustomAttribute(typeof(RedisChannelAttribute));
            if (attribute == null)
                throw new Exception(
                    $"(Redis channel attribute ({nameof(RedisChannelAttribute)}) is not set for {subscriberType.Name} type!");

            var channel = ((RedisChannelAttribute)attribute).Channel;
            await Subscribe(channel, subscriberType, cancellationToken);
        }
    }
}