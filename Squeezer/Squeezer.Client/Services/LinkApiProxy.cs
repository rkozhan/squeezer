using Squeezer.Client.Dtos;
using Squeezer.Client.Interfaces;

namespace Squeezer.Client.Services;
public class LinkApiProxy : ILinkService
{
    private readonly ILinkApi _linkApi;
    public LinkApiProxy(ILinkApi linkApi)
    {
        _linkApi = linkApi;
    }

    public Task<LinkDto> CreateLinkAsync(LinkCreateDto dto) =>
        _linkApi.CreateLinkAsync(dto);

    public Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool activeOnly) =>
        _linkApi.GetLinksByUserAsync(startIndex, pageSize, activeOnly);

    public Task<LinkDto?> UpdateLinkAsync(LinkEditDto dto) =>
        _linkApi.UpdateLinkAsync(dto.Id, dto);

    public Task DeleteLinkAsync(long id, string userId) =>
        _linkApi.DeleteLinkAsync(id);

    public Task<LinkDetailsDto?> GetLinkAsync(long id, string userId) =>
        _linkApi.GetLinkAsync(id);
    

}
