namespace March2024BackendTask.Core.Common.Abstractions;

public interface IMessageBus<TMessage>
{
    Task WriteAsync(TMessage message, CancellationToken stoppingToken);
    Task<TMessage?> ReadAsync(CancellationToken stoppingToken);
}