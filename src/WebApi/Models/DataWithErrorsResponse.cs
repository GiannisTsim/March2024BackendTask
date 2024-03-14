namespace March2024BackendTask.WebApi.Models;

public record DataWithErrorsResponse<TData>
{
    public required TData Data { get; init; }
    public Dictionary<string, string[]>? Errors { get; init; }
}