using System.Reflection;
using StackExchange.Redis;

namespace RedisStorm.Registration;

public enum SerializationType
{
    Json,
    MessagePack
}

public static class DependencyStore
{
    public static readonly Dictionary<string, Type> ChannelSubscriberDictionary = new();
    public static Assembly Assembly = null!;
    public static bool ShouldScanAssemblyForSubscribers = false;
    public static ConnectionMultiplexer? Multiplexer = null;
    public static bool MultiplexerFromServiceCollection = false;
    public static SerializationType PublishingSerializationType = SerializationType.Json;
    public static SerializationType SubscribingSerializationType = SerializationType.Json;
}