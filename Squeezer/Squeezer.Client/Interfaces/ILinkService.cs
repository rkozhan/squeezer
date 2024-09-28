using Squeezer.Client.Dtos;

namespace Squeezer.Client.Interfaces
{
    public interface ILinkService
    {
        Task<LinkDto> CreateLinkAsync(LinkCreateDto dto);

        Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool activeOnly);

        Task<LinkDto?> UpdateLinkAsync(LinkEditDto dto);

        Task DeleteLinkAsync(long id, string userId);

        Task<LinkDetailsDto?> GetLinkAsync(long id, string userId);
    }
}
