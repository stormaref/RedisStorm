namespace RedisStorm.Interfaces;

public interface ISubscriber<in TMessage>
    where TMessage : class
{
    Task Handle(TMessage message, CancellationToken cancellationToken);
}