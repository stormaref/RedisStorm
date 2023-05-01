using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Services;
using StackExchange.Redis;

namespace RedisStorm.Registration;

public class PublisherRegistrationFactory
{
    public PublisherRegistrationFactory(IServiceCollection serviceCollection)
    {
        if (DependencyStore.MultiplexerFromServiceCollection)
        {
            serviceCollection.AddScoped<IRedisPublisher, RedisPublisher>(provider =>
                new RedisPublisher(provider.GetRequiredService<ConnectionMultiplexer>()));
        }
        else
        {
            serviceCollection.AddScoped<IRedisPublisher, RedisPublisher>(_ => new RedisPublisher());
        }
    }

    public SerializationType SerializationType
    {
        set => DependencyStore.PublishingSerializationType = value;
    }
}