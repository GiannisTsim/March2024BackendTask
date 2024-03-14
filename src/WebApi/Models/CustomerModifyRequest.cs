using System.ComponentModel.DataAnnotations;

namespace March2024BackendTask.WebApi.Models;

public record CustomerModifyRequest
{
    [Required] public DateTimeOffset? UpdatedDtm { get; init; }

    [Required, MaxLength(30)] public string? FirstName { get; init; }

    [Required, MaxLength(30)] public string? LastName { get; init; }

    [Required, Range(0, 1, MinimumIsExclusive = false, MaximumIsExclusive = false)]
    public decimal? DiscountPct { get; init; }
}