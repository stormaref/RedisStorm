using RedisStorm.Attributes;
using RedisStorm.Interfaces;
using RedisStorm.Test.Messages;

namespace RedisStorm.Test.Subscribers;

[RedisChannel("second-sample-channel")]
public class SecondSampleSubscriber : ISubscriber<SampleMessage>
{
    public Task Handle(SampleMessage message, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}