﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace RedisStorm.Registration;

public class RedisStormRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;

    public RedisStormRegistrationFactory(IServiceCollection serviceCollection, Assembly assembly)
    {
        _serviceCollection = serviceCollection;
        DependencyStore.Assembly = assembly;
    }

    public void AddSubscribers(Action<SubscriberRegistrationFactory> srf)
    {
        srf.Invoke(new SubscriberRegistrationFactory(_serviceCollection));
    }
}