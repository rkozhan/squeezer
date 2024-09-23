using Squeezer.Client.Dtos;
using Squeezer.Client.Extensions;
using Squeezer.Client.Interfaces;
using Squeezer.Services;
using System.Security.Claims;

namespace Squeezer.Endpoints;

public static class LinkEndpoints
{ 
    public static IEndpointRouteBuilder MapLinkEndpoints(this IEndpointRouteBuilder app)
    {
        var linksGroup = app.MapGroup("/api/links").RequireAuthorization();

        linksGroup.MapPost("", async (LinkCreateDto dto, ILinkService linkService, ClaimsPrincipal principal) =>
            {
                var userId = principal.GetUserId();

                if (userId != dto.UserId)
                    return Results.Unauthorized();

                var link = await linkService.CreateLinkAsync(dto);
                return Results.Ok(link);
            }
        );

        linksGroup.MapGet("", async (int startIndex, int pageSize, bool activeOnly, ILinkService linkService, ClaimsPrincipal principal) =>
        {
            var userId = principal.GetUserId();
            var pagedResult = await linkService.GetLinksByUserAsync(userId, startIndex, pageSize, activeOnly);
            return Results.Ok(pagedResult);
        });

        return app;
    }
}

