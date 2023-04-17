using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Services;

namespace RedisStorm.Registration;

public class PublisherRegistrationFactory
{
    public PublisherRegistrationFactory(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IRedisPublisher, RedisPublisher>();
    }

    public SerializationType SerializationType
    {
        set => DependencyStore.PublishingSerializationType = value;
    }
}