using System.Text.Json;
using MessagePack;
using RedisStorm.Exceptions;
using RedisStorm.Registration;
using StackExchange.Redis;

namespace RedisStorm.Services;

public interface IRedisPublisher
{
    Task Publish<TMessage>(string channel, TMessage message);
}

public class RedisPublisher : IRedisPublisher
{
    private readonly ISubscriber _subscriber;

    public RedisPublisher()
    {
        _subscriber = DependencyStore.Multiplexer?.GetSubscriber() ?? throw new ConnectionMultiplexerException();
    }

    public RedisPublisher(IConnectionMultiplexer multiplexer)
    {
        _subscriber = multiplexer.GetSubscriber();
    }

    public async Task Publish<TMessage>(string channel, TMessage message)
    {
        switch (DependencyStore.PublishingSerializationType)
        {
            case SerializationType.Json:
                await PublishJson(channel, message);
                break;
            case SerializationType.MessagePack:
                await PublishMessagePack(channel, message);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task PublishMessagePack<TMessage>(string channel, TMessage message)
    {
        var pack = Convert.ToBase64String(MessagePackSerializer.Serialize(message));
        await _subscriber.PublishAsync(channel, pack);
    }

    private async Task PublishJson<TMessage>(string channel, TMessage message)
    {
        await _subscriber.PublishAsync(channel, JsonSerializer.Serialize(message));
    }
}