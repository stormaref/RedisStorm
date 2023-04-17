using Microsoft.Extensions.DependencyInjection;

namespace RedisStorm.Registration;

public class RedisStormRegistrationFactory
{
    private readonly IServiceCollection _serviceCollection;

    public RedisStormRegistrationFactory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    
    
}