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
}
