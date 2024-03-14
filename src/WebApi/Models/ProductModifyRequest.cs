using System.ComponentModel.DataAnnotations;

namespace March2024BackendTask.WebApi.Models;

public record ProductModifyRequest
{
    [Required] public DateTimeOffset? UpdatedDtm { get; init; }

    [Required, MaxLength(30)] public string? Name { get; init; }

    [Required, MaxLength(100)] public string? Description { get; init; }

    [Required, Range(0, 99999, MinimumIsExclusive = true, MaximumIsExclusive = false)]
    public decimal? Price { get; init; }
}