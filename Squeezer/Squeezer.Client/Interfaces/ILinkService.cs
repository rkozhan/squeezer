using Squeezer.Client.Dtos;

namespace Squeezer.Client.Interfaces
{
    public interface ILinkService
    {
        Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);

        Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool activeOnly);
    }
}
