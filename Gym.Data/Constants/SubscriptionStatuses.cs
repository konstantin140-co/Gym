namespace Gym.Data.Constants;

public static class SubscriptionStatuses
{
    public const string Active = "Активен";
    public const string Expired = "Истёк";
    public const string Cancelled = "Отменён";

    public static readonly string[] All = [Active, Expired, Cancelled];
}
