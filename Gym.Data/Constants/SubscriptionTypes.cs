namespace Gym.Data.Constants;

public static class SubscriptionTypes
{
    public const string Month = "30 дней";
    public const string Quarter = "90 дней";
    public const string Year = "365 дней";

    public static readonly string[] All = [Month, Quarter, Year];

    public static int GetDays(string type) => type switch
    {
        Month => 30,
        Quarter => 90,
        Year => 365,
        _ => 30
    };

    public static decimal GetPrice(string type) => type switch
    {
        Month => 2500m,
        Quarter => 6500m,
        Year => 22000m,
        _ => 2500m
    };
}
