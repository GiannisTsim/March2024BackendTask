using March2024BackendTask.Core.Common.Abstractions;
using March2024BackendTask.Core.Purchase.Abstractions;
using March2024BackendTask.Core.Purchase.Exceptions;
using March2024BackendTask.Core.Purchase.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace March2024BackendTask.Infrastructure.Services;

public class PurchaseNotificationService : BackgroundService
{
    private readonly IPurchaseRepository _repository;
    private readonly IMessageBus<PurchaseNotifyCommand> _messageBus;
    private readonly ILogger<PurchaseNotificationService> _logger;

    public PurchaseNotificationService(
        IPurchaseRepository repository,
        ILogger<PurchaseNotificationService> logger,
        IMessageBus<PurchaseNotifyCommand> messageBus
    )
    {
        _repository = repository;
        _logger = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"{nameof(PurchaseNotificationService)} is starting.");
        stoppingToken.Register
        (
            () => _logger.LogDebug($"{nameof(PurchaseNotificationService)} detected cancellation request.")
        );

        await InitializeWorkAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"{nameof(PurchaseNotificationService)} doing background work.");
            var message = await _messageBus.ReadAsync(stoppingToken);
            if (message != null) await DispatchNotificationAsync(message);
        }

        _logger.LogDebug($"{nameof(PurchaseNotificationService)} is stopping.");
    }


    /// <summary>
    /// Initializes the message queue with any submitted purchases pending notifications
    /// </summary>
    private async Task InitializeWorkAsync(CancellationToken stoppingToken)
    {
        var purchases = await _repository.GetPendingNotificationsAsync();
        foreach (var purchase in purchases)
        {
            if (stoppingToken.IsCancellationRequested) break;
            var command = new PurchaseNotifyCommand(purchase.CustomerNo, purchase.PurchaseDtm);
            await _messageBus.WriteAsync(command, stoppingToken);
        }
    }

    
    /// <summary>
    /// Handles the notification request and updates the database to reflect the new notification state
    /// </summary>
    private async Task DispatchNotificationAsync(PurchaseNotifyCommand command)
    {
        _logger.LogInformation
        (
            "Customer '{CustomerNo}' successfully notified for purchase '{PurchaseDtm}'.",
            command.CustomerNo,
            command.PurchaseDtm
        );

        try
        {
            await _repository.AddNotificationAsync(command);
        }
        catch (Exception ex) when (ex is PurchaseNotYetSubmittedException or PurchaseAlreadyNotifiedException)
        {
            _logger.LogError(ex, "Invalid notification message detected.");
        }
    }
}