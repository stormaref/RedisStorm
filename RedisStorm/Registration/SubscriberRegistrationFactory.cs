using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Extensions;
using RedisStorm.Interfaces;
using RedisStorm.Services;
using StackExchange.Redis;
using ISubscriber = RedisStorm.Interfaces.ISubscriber;

namespace RedisStorm.Registration;

public class SubscriberRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;

    public SubscriberRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
        if (DependencyStore.MultiplexerFromServiceCollection)
        {
            _serviceCollection.AddHostedService<RedisHostedService>(provider =>
                new RedisHostedService(provider.GetRequiredService<IServiceScopeFactory>(),
                    provider.GetRequiredService<ConnectionMultiplexer>()));
        }
        else
        {
            _serviceCollection.AddHostedService<RedisHostedService>(provider =>
                new RedisHostedService(provider.GetRequiredService<IServiceScopeFactory>()));
        }
    }

    public SerializationType SerializationType
    {
        set => DependencyStore.SubscribingSerializationType = value;
    }

    public void ConfigSubscriber<TSubscriber>(string channelName)
        where TSubscriber : class, ISubscriber
    {
        var subscriberType = typeof(TSubscriber);
        if (string.IsNullOrWhiteSpace(channelName))
            throw new ArgumentException($"channel name is null or empty for {subscriberType.Name}",
                nameof(channelName));

        var messageType = subscriberType.GetMessageTypeOfSubscriberType();
        _serviceCollection.AddScoped(typeof(ISubscriber<>).MakeGenericType(messageType), subscriberType);
        DependencyStore.ChannelSubscriberDictionary[channelName] = subscriberType;
    }

    public void AddSubscribersFromAssembly()
    {
        var types = DependencyStore.Assembly
            .GetTypes()
            .Where(t => t.IsClass &&
                        t.GetInterfaces().Contains(typeof(ISubscriber)))
            .ToList();

        foreach (var subscriberType in types)
        {
            var messageType = subscriberType.GetMessageTypeOfSubscriberType();
            _serviceCollection.AddScoped(typeof(ISubscriber<>).MakeGenericType(messageType), subscriberType);
        }

        DependencyStore.ShouldScanAssemblyForSubscribers = true;
    }
}