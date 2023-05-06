# RedisStorm

This is a simple wrapper for StackExchange.Redis that would make using redis pub sub easier.

# Features
- You can use message pack serialization for better performance
- Adding subscribers automatically and binding them to channels with attribute
- Using built-in DI container for handling messages so that you can inject services inside your subscriber (message handler)

# Installation
## Using package manager:
```
Install-Package RedisStorm -Version 1.2.2
```

# Usage/Examples
## Setup
```csharp
        services.AddRedisStorm(Assembly.GetExecutingAssembly(), factory =>
        {
        
            //Add multiplexer below
            factory.AddConnectionMultiplexer(multiplexer);
            // factory.AddConnectionMultiplexerFromServiceCollection();
            
            factory.AddPublisher(registrationFactory =>
            {
                registrationFactory.SerializationType = SerializationType.MessagePack;
            });

            factory.AddSubscribers(registrationFactory =>
            {
                //You can change type to message pack here (default is json)
                registrationFactory.SerializationType = SerializationType.MessagePack;
                
                //You can use below line for automaticaly registring subscribers
                registrationFactory.AddSubscribersFromAssembly(); 
                
                //Also you can add subscribers here manually with this method
                registrationFactory.ConfigSubscriber<SampleSubscriber>("channelName");
            });
        });
```

## Subscriber
```csharp
namespace RedisStorm.Test.Subscribers;

[RedisChannel("sample-channel")]
public class SampleSubscriber : ISubscriber<SampleMessage>
{
    public Task Handle(SampleMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
```
> You should use this attribute only when you want to bind subscriber automatically

## Publisher
```csharp
namespace RedisStorm.Test.Services;

private readonly IRedisPublisher _publisher;  
  
public TestService(IRedisPublisher publisher)  
{  
	_publisher = publisher;  
}

public async Task Test()
{
    _publisher.Publish("channel", new SampleMessage());
}
```
> It only works when you call ***AddPublisher*** method in ***AddRedisStorm***
