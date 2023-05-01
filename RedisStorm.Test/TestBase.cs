using System.Net;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Extensions;
using RedisStorm.Registration;
using RedisStorm.Test.Subscribers;
using StackExchange.Redis;

namespace RedisStorm.Test;

public class TestBase
{
    public TestBase()
    {
        var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
        {
            EndPoints = new EndPointCollection(new List<EndPoint>
            {
                new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6379)
            })
        });

        ServiceCollection serviceCollection = new();

        serviceCollection.AddSingleton(multiplexer);

        serviceCollection.AddRedisStorm(Assembly.GetExecutingAssembly(), factory =>
        {
            factory.AddPublisher(registrationFactory =>
            {
                registrationFactory.SerializationType = SerializationType.MessagePack;
            });

            // factory.AddConnectionMultiplexer(multiplexer);
            factory.AddConnectionMultiplexerFromServiceCollection();

            factory.AddSubscribers(registrationFactory =>
            {
                registrationFactory.SerializationType = SerializationType.MessagePack;
                registrationFactory.AddSubscribersFromAssembly();
                registrationFactory.ConfigSubscriber<SampleSubscriber>("channel");
            });
        });
    }
}