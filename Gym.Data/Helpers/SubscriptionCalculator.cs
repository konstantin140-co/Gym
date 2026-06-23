using Gym.Data.Constants;
using Gym.Data.Models;

namespace Gym.Data.Helpers;

public static class SubscriptionCalculator
{
    public static DateTime CalculateEndDate(string type, DateTime startDate)
        => startDate.Date.AddDays(SubscriptionTypes.GetDays(type));

    public static bool IsActive(Subscription subscription, DateTime? asOf = null)
    {
        var now = (asOf ?? DateTime.Now).Date;
        return subscription.Status == SubscriptionStatuses.Active
               && subscription.StartDate.Date <= now
               && subscription.EndDate.Date >= now;
    }

    public static int DaysUntilExpiry(Subscription subscription, DateTime? asOf = null)
    {
        var now = (asOf ?? DateTime.Now).Date;
        return (subscription.EndDate.Date - now).Days;
    }

    public static bool IsExpiringSoon(Subscription subscription, int days = 7, DateTime? asOf = null)
    {
        if (!IsActive(subscription, asOf)) return false;
        var left = DaysUntilExpiry(subscription, asOf);
        return left >= 0 && left < days;
    }
}
