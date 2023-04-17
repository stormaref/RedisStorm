using RedisStorm.Attributes;
using RedisStorm.Interfaces;
using RedisStorm.Test.Messages;

namespace RedisStorm.Test.Subscribers;

[RedisChannel("sample-channel")]
public class SampleSubscriber : ISubscriber<SampleMessage>
{
    public Task Handle(SampleMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}