using System.Reflection;

namespace RedisStorm.Registration;

public static class DependencyStore
{
    public static readonly Dictionary<string, Type> ChannelSubscriberDictionary = new();
    public static Assembly Assembly = null!;
    public static bool ShouldScanAssemblyForSubscribers = false;
}