using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace RedisStorm.Services;

public class RedisHostedService : IHostedService
{
    private readonly ConnectionMultiplexer _multiplexer;

    public RedisHostedService(ConnectionMultiplexer multiplexer)
    {
        _multiplexer = multiplexer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var channelName = "";
        await _multiplexer.GetSubscriber().SubscribeAsync(channelName, (channel, message) =>
        {
            if (!message.HasValue)
            {
                return;
            }

            var rawJson = (string)message!;
        });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}