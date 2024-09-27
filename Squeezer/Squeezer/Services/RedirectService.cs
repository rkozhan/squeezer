using Microsoft.EntityFrameworkCore;
using Squeezer.Data;

namespace Squeezer.Services;

public interface IRedirectService
{
    Task<string?> GetLongUrlByShortCodeAsync(string shortCode);
}


public class RedirectService : IRedirectService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public RedirectService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task<string?> GetLongUrlByShortCodeAsync(string shortCode)
    {
        await using var context = _contextFactory.CreateDbContext();

        var link = context.Links.FirstOrDefault(l => l.ShortCode == shortCode);
        if (link == null) return null;

        var linkAnalytic = new LinkAnalytic
        {
            LinkId = link.Id,
            ClicedAt = DateTime.UtcNow
        };

        context.LinkAnalytics.Add(linkAnalytic);
        await context.SaveChangesAsync();

        return link.LongUrl;
    }
}
