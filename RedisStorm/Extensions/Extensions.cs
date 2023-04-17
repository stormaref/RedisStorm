using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RedisStorm.Registration;

namespace RedisStorm.Extensions;

public static class Extensions
{
    public static void AddRedisStorm(this IServiceCollection serviceCollection, Assembly assembly,
        Action<RedisStormRegistrationFactory> action)
    {
        action.Invoke(new RedisStormRegistrationFactory(serviceCollection, assembly));
    }

    internal static Type GetMessageTypeOfSubscriberType(this Type subscriberType)
    {
        return subscriberType.GetInterface("ISubscriber`1")!.GetGenericArguments().First();
    }
}