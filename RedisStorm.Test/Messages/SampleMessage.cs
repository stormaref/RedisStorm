namespace RedisStorm.Test.Messages;

public class SampleMessage
{
    public SampleMessage(string title)
    {
        Title = title;
    }
    public string Title { get; set; }   
}