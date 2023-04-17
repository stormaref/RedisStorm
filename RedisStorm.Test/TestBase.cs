using System.Net;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Extensions;
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
        serviceCollection.AddRedisStorm(Assembly.GetExecutingAssembly(), factory =>
        {
            factory.AddSubscribers(registrationFactory =>
            {
                registrationFactory.AddConnectionMultiplexer(multiplexer);
                registrationFactory.AddSubscribersFromAssembly();
                // registrationFactory.ConfigSubscriber<SampleSubscriber>("channel");
            });
        });
    }
}