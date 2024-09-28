﻿using Refit;
using Squeezer.Client.Dtos;
namespace Squeezer.Client.Services;
public interface ILinkApi
{
    [Post("/api/links")]
    Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);

    [Get("/api/links")]
    Task<PagedResult<LinkDto>> GetLinksByUserAsync(int startIndex, int pageSize, bool activeOnly);

    [Patch("/api/links/{linkId}")]
    Task<LinkDto?> UpdateLinkAsync(long linkId, LinkEditDto dto);

    [Delete("/api/links/{linkId}")]
    Task DeleteLinkAsync(long linkId);

    [Get("/api/links/{linkId}")]
    Task<LinkDetailsDto?> GetLinkAsync(long linkId);
}

