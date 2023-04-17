using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Interfaces;

namespace RedisStorm.Registration;

public class SubscriberRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;

    public SubscriberRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public void AddSubscriber<TSubscriber, TMessage>(string channelName)
        where TMessage : class
        where TSubscriber : class, ISubscriber<TMessage>
    {
        if (string.IsNullOrWhiteSpace(channelName))
        {
            throw new ArgumentException($"channel name is null or empty for {typeof(TSubscriber).Name}",
                nameof(channelName));
        }

        _serviceCollection.AddScoped<ISubscriber<TMessage>, TSubscriber>();
        DependencyStore.ChannelSubscriberDictionary[channelName] = typeof(TSubscriber);
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
            var messageType = subscriberType.GetGenericArguments().First();
            _serviceCollection.AddScoped(typeof(ISubscriber<>).MakeGenericType(messageType), subscriberType);
        }

        DependencyStore.ShouldScanAssemblyForSubscribers = true;
    }
}