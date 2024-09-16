using Squeezer.Client.Dtos;
using Squeezer.Client.Extensions;
using Squeezer.Client.Interfaces;
using System.Security.Claims;

namespace Squeezer.Endpoints;

public static class LinkEndpoints
{ 
    public static IEndpointRouteBuilder MapLinkEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/links", async (LinkCreateDto dto, ILinkService linkService, ClaimsPrincipal principal) =>
        {
            var userId = principal.GetUserId();

            if (userId != dto.UserId)
                return Results.Unauthorized();

            var link = await linkService.CreateLinkAsync(dto);
            return Results.Ok(link);
        }
        ).RequireAuthorization();

        return app;
    }
}

