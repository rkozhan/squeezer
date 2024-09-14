using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Squeezer.Client.Dtos;
using Squeezer.Data;

namespace Squeezer.Services
{
    public class LinkService
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
                ShortUrl = domain,
                UserId = dto.UserId,
                IsActive = true,
            };

            await using var context = _contextFactory.CreateDbContext();
            await context.Links.AddAsync(link);

            return new LinkDto
            {
                Id = link.Id,
                LongUrl = link.LongUrl,
                IsActive = link.IsActive,
                ShortUrl = link.ShortUrl,
            };
        }
    }
}
