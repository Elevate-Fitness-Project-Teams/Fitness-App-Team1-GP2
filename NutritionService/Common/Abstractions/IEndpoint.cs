namespace NutritionService.Common.Abstractions;

/// <summary>
/// Every vertical slice exposes its own endpoint mapping through this interface.
/// Program.cs discovers and maps all of them via reflection — no central "routes" file
/// to keep growing, each feature stays self-contained.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
