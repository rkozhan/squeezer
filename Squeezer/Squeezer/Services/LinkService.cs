using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Squeezer.Client.Dtos;
using Squeezer.Data;

namespace Squeezer.Services{
    public class LinkService : Client.Interfaces.ILinkService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IShortCodeGeneratorService _shortCodeGeneratorService;
        private readonly IConfiguration _configuration;

        public LinkService(IDbContextFactory<ApplicationDbContext> contextFactory,
            IShortCodeGeneratorService shortCodeGeneratorService,
            IConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _shortCodeGeneratorService = shortCodeGeneratorService;
            _configuration = configuration;
        }

        public async Task<LinkDto> CreateLinkAsync(LinkCreateDto dto)
        {
            var domain = _configuration["Domain"] ?? throw new InvalidOperationException($"Domain is not defined in appsettings");
            var shortCode = await _shortCodeGeneratorService.GenerateShortCodeAsync();

            var link = new Link
            {
                LongUrl = dto.LongUrl,
                ShortCode = shortCode,
                ShortUrl = $"{domain.TrimEnd('/')}/{shortCode}",
                UserId = dto.UserId,
                IsActive = true,
            };

            await using var context = _contextFactory.CreateDbContext();
            await context.Links.AddAsync(link);
            await context.SaveChangesAsync();

            return new LinkDto
            {
                Id = link.Id,
                LongUrl = link.LongUrl,
                IsActive = link.IsActive,
                ShortUrl = link.ShortUrl,
            };
        }

        public async Task<PagedResult<LinkDto>> GetLinksByUserAsync(string userId, int startIndex, int pageSize, bool activeOnly)
        {
            await using var context = _contextFactory.CreateDbContext();

            var query = context.Links.Where(l => l.UserId == userId);
            if (activeOnly)
            {
                query = query.Where(l => l.IsActive);
            }

            var totalCount = await query.CountAsync();
            var links = await query.Skip(startIndex)
                .Take(pageSize)
                .Select(l => new LinkDto
                {
                    Id=l.Id,
                    LongUrl = l.LongUrl,
                    IsActive=l.IsActive,
                    ShortUrl = l.ShortUrl,
                    TotalClicks = l.linkAnalytics.Count,
                })
                .ToArrayAsync();

            return new PagedResult<LinkDto>(links, totalCount);
        }
    }
}
