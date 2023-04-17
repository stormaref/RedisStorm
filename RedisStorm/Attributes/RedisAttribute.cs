namespace RedisStorm.Attributes;

public class RedisChannelAttribute : Attribute
{
    public RedisChannelAttribute(string channel)
    {
        Channel = channel;
    }

    public string Channel { get; init; }
}