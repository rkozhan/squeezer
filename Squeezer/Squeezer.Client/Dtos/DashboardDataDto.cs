using System.Numerics;

namespace Squeezer.Client.Dtos;

public record DashboardDataDto(
    int TotalLinks,
    int TotalClicks,
    int TotalActiveLinks,
    int TotalInactiveLinks,
    double AverageClicksPerLink,
    int TotalClicksToday
);

