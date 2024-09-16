using Refit;
using Squeezer.Client.Dtos;
namespace Squeezer.Client.Services;
public interface ILinkApi
{
    [Post("/api/links")]
    Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);
}

