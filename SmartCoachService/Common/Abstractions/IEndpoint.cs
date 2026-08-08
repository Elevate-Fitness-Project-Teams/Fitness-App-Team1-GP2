namespace SmartCoachService.Common.Abstractions;

/// <summary>Every vertical slice maps its own route; Program.cs discovers them via reflection.</summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
