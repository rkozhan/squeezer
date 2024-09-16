using Squeezer.Client.Dtos;

namespace Squeezer.Client.Interfaces
{
    public interface ILinkService
    {
        Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);
    }
}
