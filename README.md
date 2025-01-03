
# 🚀 **RedisStorm**: A Simple Redis Pub/Sub Wrapper for .NET

RedisStorm is a powerful, easy-to-use wrapper for **StackExchange.Redis** that simplifies working with **Redis Pub/Sub**. Perfect for anyone who needs reliable messaging, streamlined integration, and better performance! 🚀

## 🎉 **Key Features**
- **✨ MessagePack Serialization**: Use MessagePack for better performance and more efficient data handling.  
- **📡 Auto-subscribing with Attributes**: Automatically bind your subscribers to channels using custom attributes.  
- **🔧 Built-in DI Support**: Inject services directly into your message handlers (subscribers) for cleaner, more maintainable code.

## 💡 **Installation**

### Using the Package Manager:
Simply run the following command to install RedisStorm via NuGet:
```bash
Install-Package RedisStorm -Version 9.0.0
```

## ⚙️ **Setup/Usage**

### Step 1: Configuration
Set up the `RedisStorm` factory with a connection multiplexer and configure publishers and subscribers:
```csharp
services.AddRedisStorm(Assembly.GetExecutingAssembly(), factory =>
{
    // Add Redis connection multiplexer
    factory.AddConnectionMultiplexer(multiplexer);
    
    // Add publisher configuration
    factory.AddPublisher(registrationFactory =>
    {
        registrationFactory.SerializationType = SerializationType.MessagePack; // Choose serialization type
    });

    // Add subscribers configuration
    factory.AddSubscribers(registrationFactory =>
    {
        registrationFactory.SerializationType = SerializationType.MessagePack; // Use MessagePack for serialization
        registrationFactory.AddSubscribersFromAssembly(); // Auto-register all subscribers in assembly
        registrationFactory.ConfigSubscriber<SampleSubscriber>("channelName"); // Manually add a subscriber
    });
});
```

### Step 2: Subscriber Example
A subscriber that listens for messages on a Redis channel. Automatically bind to the channel using the `RedisChannel` attribute.
```csharp
[RedisChannel("sample-channel")]
public class SampleSubscriber : ISubscriber<SampleMessage>
{
    public Task Handle(SampleMessage message, CancellationToken cancellationToken)
    {
        // Handle message logic
        throw new NotImplementedException();
    }
}
```
> **Tip**: Use the `RedisChannel` attribute for **automatic** binding of subscribers to channels.

### Step 3: Publisher Example
Publish messages to the Redis channel using the `IRedisPublisher` interface.  
```csharp
public class TestService
{
    private readonly IRedisPublisher _publisher;

    public TestService(IRedisPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishMessage()
    {
        await _publisher.Publish("channel", new SampleMessage());
    }
}
```

> **Note**: Ensure you call `AddPublisher` when setting up RedisStorm to enable publishing!

---

## 🚀 Why Use RedisStorm?

🔧 **Automated Integration**: Subscribers are automatically added to your channels, saving you time on boilerplate code.  
⚡ **High Performance**: Use MessagePack for serialization, making your Redis communication faster and more efficient.  
💬 **Seamless Dependency Injection**: Easily inject services into your subscribers, making the code clean and reusable.

---

Feel free to reach out or contribute to make **RedisStorm** even better! 🙌
