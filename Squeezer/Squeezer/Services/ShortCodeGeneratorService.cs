using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Squeezer.Data;

namespace Squeezer.Services;

public interface IShortCodeGeneratorService
{
    Task<string> GenerateShortCodeAsync();
}

public class ShortCodeGeneratorService : IShortCodeGeneratorService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    public ShortCodeGeneratorService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
    public async Task<string> GenerateShortCodeAsync()
    {
        var shortCode = GenerateShortCode(6);

        await using var context = _contextFactory.CreateDbContext();

        while (await context.Links.AsNoTracking()
            .AnyAsync(l => l.ShortCode == shortCode))
        {
            shortCode = GenerateShortCode(6);
        }

        return shortCode;
    }

    private static string GenerateShortCode(int length)
    {
        const string availableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var shortCodeChars = Enumerable.Repeat(availableChars, length)
            .Select(s =>
            {
                var randomNumber = Random.Shared.Next(s.Length);
                return s[randomNumber];
            }).ToArray();
        return new string(shortCodeChars);
    }
}


