using March2024BackendTask.Core.Purchase.Models;

namespace March2024BackendTask.Core.Purchase.Abstractions;

public interface IPurchaseRepository
{
    Task<IEnumerable<PurchaseSimple>> GetAsync(int? customerNo = null);
    Task<IEnumerable<PurchaseSimple>> GetPendingNotificationsAsync();
    Task<PurchaseDetails?> GetDetailsAsync(int customerNo, DateTimeOffset purchaseDtm);
    Task AddItemAsync(PurchaseItemAddCommand command);
    Task AddItemBatchAsync(IEnumerable<PurchaseItemAddCommand> commands);
    Task AddSubmitAsync(PurchaseSubmitCommand command);
    Task AddNotificationAsync(PurchaseNotifyCommand command);
}