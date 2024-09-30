using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Squeezer.Client.Dtos;
using Squeezer.Data;

namespace Squeezer.Services;

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
            ShortUrl = $"{domain.TrimEnd('/')}/r/{shortCode}",
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
        var links = await query.OrderByDescending(l => l.Id)
            .Skip(startIndex)
            .Take(pageSize)
            .Select(l => new LinkDto
            {
                Id = l.Id,
                LongUrl = l.LongUrl,
                IsActive = l.IsActive,
                ShortUrl = l.ShortUrl,
                TotalClicks = l.linkAnalytics.Count,
            })
            .ToArrayAsync();

        return new PagedResult<LinkDto>(links, totalCount);
    }

    public async Task<LinkDto?> UpdateLinkAsync(LinkEditDto dto)
    {
        await using var context = _contextFactory.CreateDbContext();
        var dbLink = await context.Links
            .FirstOrDefaultAsync(l => l.Id == dto.Id && l.UserId == dto.UserId);

        if (dbLink == null) return null;

        dbLink.LongUrl = dto.LongUrl;
        dbLink.IsActive = dto.isActive;
        context.Links.Update(dbLink);

        await context.SaveChangesAsync();
        return new LinkDto
        {
            Id = dto.Id,
            LongUrl = dto.LongUrl,
            IsActive = dto.isActive,
            ShortUrl = dbLink.ShortUrl,
        };
    }

    public async Task DeleteLinkAsync(long id, string userId)
    {
        await using var context = _contextFactory.CreateDbContext();

        var link = await context.Links
            .Include(l => l.linkAnalytics)
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

        if (link == null) return;

        if(link.linkAnalytics.Count > 0) context.LinkAnalytics.RemoveRange(link.linkAnalytics);
        context.Links.Remove(link);

        await context.SaveChangesAsync();
    }

    public async Task<LinkDetailsDto?> GetLinkAsync(long id, string userId)
    {
        await using var context = _contextFactory.CreateDbContext();

        var link = await context.Links
            .Include(l => l.linkAnalytics)
            .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

        if (link == null) return null;

        LinkAnaliticDto[] linkAnalytics = link.linkAnalytics ?
            .Select(a => new LinkAnaliticDto
        {
            Id = a.Id,
            ClickedAt = a.ClicedAt,
            LinkId = a.LinkId
        }).ToArray()
        ?? [];

        var linkDto = new LinkDto
        {
            Id = id,
            IsActive = link.IsActive,
            LongUrl = link.LongUrl,
            ShortUrl = link.ShortUrl,
            TotalClicks = linkAnalytics.Length
        };
        return new LinkDetailsDto(linkDto, linkAnalytics);
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync(string userId)
    {
        //await using var context = _contextFactory.CreateDbContext();
        //var totalLinks = await context.Links.CountAsync(l => l.UserId == userId);
        //var totalActiveLinks = await context.Links.CountAsync(l => l.UserId == userId && l.IsActive);
        //var totalClicks = await context.LinkAnalytics.CountAsync(a => a.OriginalLink.UserId == userId);
        //var totalClicksToday = await context.LinkAnalytics.CountAsync(a => a.OriginalLink.UserId == userId && a.ClicedAt.Date == DateTime.Today);

        var counts = await Task.WhenAll(
            GetTotalLinks(userId),
            GetTotalClicks(userId),
            GetTotalActiveLinks(userId),
            GetTotalClicksToday(userId)
        );

        var totalLinks = counts[0];
        var totalClicks = counts[1];
        var totalActiveLinks = counts[2];
        var totalClicksToday = counts[3];

        var totalInactiveLinks = totalLinks - totalActiveLinks;
        double averageClicksPerLink = totalLinks > 0 ? (double)totalClicks / totalLinks : 0;

        return new DashboardDataDto(totalLinks, totalClicks, totalActiveLinks, totalInactiveLinks, averageClicksPerLink, totalClicksToday);
    }

    private async Task<int> GetTotalLinks(string userId)
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.Links.CountAsync(l => l.UserId == userId);
    }

    private async Task<int> GetTotalClicks(string userId)
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.LinkAnalytics.CountAsync(a => a.OriginalLink.UserId == userId);
    }

    private async Task<int> GetTotalActiveLinks(string userId)
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.Links.CountAsync(l => l.UserId == userId && l.IsActive);
    }

    private async Task<int> GetTotalClicksToday(string userId)
    {
        await using var context = _contextFactory.CreateDbContext();
        var today = DateTime.UtcNow.Date;
        return await context.LinkAnalytics.CountAsync(a => a.OriginalLink.UserId == userId && a.ClicedAt.Date == today);
    }
}
