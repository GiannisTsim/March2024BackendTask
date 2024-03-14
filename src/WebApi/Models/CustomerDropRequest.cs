using System.ComponentModel.DataAnnotations;

namespace March2024BackendTask.WebApi.Models;

public record CustomerDropRequest
{
    [Required] public DateTimeOffset? UpdatedDtm { get; init; }
}