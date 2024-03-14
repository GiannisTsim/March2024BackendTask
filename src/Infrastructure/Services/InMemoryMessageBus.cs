using System.Threading.Channels;
using March2024BackendTask.Core.Common.Abstractions;

namespace March2024BackendTask.Infrastructure.Services;

public class InMemoryMessageBus<TMessage> : IMessageBus<TMessage>
{
    private readonly Channel<TMessage> _channel;

    public InMemoryMessageBus()
    {
        var options = new UnboundedChannelOptions
        {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = false
        };
        _channel = Channel.CreateUnbounded<TMessage>(options);
    }

    public async Task WriteAsync(TMessage message, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && await _channel.Writer.WaitToWriteAsync(stoppingToken))
        {
            if (_channel.Writer.TryWrite(message)) return;
        }
    }

    public async Task<TMessage?> ReadAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && await _channel.Reader.WaitToReadAsync(stoppingToken))
        {
            if (_channel.Reader.TryRead(out var message))
            {
                return message;
            }
        }
        return default;
    }
}