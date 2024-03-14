using System.ComponentModel.DataAnnotations;

namespace March2024BackendTask.WebApi.Models;

public record ProductDropRequest
{
    [Required] public DateTimeOffset? UpdatedDtm { get; init; }
}