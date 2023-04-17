namespace RedisStorm.Interfaces;

public interface ISubscriber
{
}

public interface ISubscriber<in TMessage> : ISubscriber
    where TMessage : class
{
    Task Handle(TMessage message, CancellationToken cancellationToken);
}