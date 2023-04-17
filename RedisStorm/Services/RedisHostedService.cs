using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisStorm.Attributes;
using RedisStorm.Extensions;
using RedisStorm.Interfaces;
using RedisStorm.Registration;
using StackExchange.Redis;

namespace RedisStorm.Services;

public class RedisHostedService : IHostedService
{
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly IServiceScopeFactory _scopeFactory;

    public RedisHostedService(IServiceScopeFactory scopeFactory)
    {
        _multiplexer = DependencyStore.Multiplexer ?? throw new Exception("Connection multiplexer is not set!");
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await GetSubscribersFromStore();
        if (DependencyStore.ShouldScanAssemblyForSubscribers)
        {
            await GetSubscribersFromAssembly();
        }
    }

    private async Task GetSubscribersFromStore()
    {
        foreach (var channel in DependencyStore.ChannelSubscriberDictionary.Keys)
        {
            await Subscribe(channel, DependencyStore.ChannelSubscriberDictionary[channel]);
        }
    }

    private async Task Subscribe(string channelName, Type subscriberType)
    {
        var messageType = subscriberType.GetMessageTypeOfSubscriberType();

        using var scope = _scopeFactory.CreateScope();
        var service = scope.ServiceProvider
            .GetRequiredService(typeof(ISubscriber<>)
                .MakeGenericType(messageType));

        await _multiplexer.GetSubscriber().SubscribeAsync(channelName, async (channel, message) =>
        {
            if (!message.HasValue)
            {
                return;
            }

            var rawJson = (string)message!;
            var obj = JsonSerializer.Deserialize(rawJson, messageType);
            await (Task)subscriberType.GetMethod("Handle")!.Invoke(service, new[] { obj })!;
        });
    }

    private async Task GetSubscribersFromAssembly()
    {
        var types = DependencyStore.Assembly
            .GetTypes()
            .Where(t => t.IsClass &&
                        t.GetInterfaces().Contains(typeof(Interfaces.ISubscriber)))
            .ToList();

        foreach (var subscriberType in types)
        {
            var attribute = subscriberType.GetCustomAttribute(typeof(RedisChannelAttribute));
            if (attribute == null)
            {
                throw new Exception(
                    $"(Redis channel attribute ({nameof(RedisChannelAttribute)}) is not set for {subscriberType.Name} type!");
            }

            var channel = ((RedisChannelAttribute)attribute).Channel;
            await Subscribe(channel, subscriberType);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}