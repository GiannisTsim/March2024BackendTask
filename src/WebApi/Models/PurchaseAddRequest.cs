using System.ComponentModel.DataAnnotations;

namespace March2024BackendTask.WebApi.Models;

public record PurchaseAddRequest
{
    public record PurchaseItem
    {
        [Required] public int? ProductNo { get; init; }
        [Required, Range(1, int.MaxValue)] public int? Quantity { get; init; }
    }
    
    public DateTimeOffset? PurchaseDtm { get; init; }
    [Required, MinLength(1)] public PurchaseItem[]? Items { get; init; }
}