using Gym.Data.Constants;
using Gym.Data.Helpers;
using Gym.Data.Models;

namespace Gym.Tests;

public class SubscriptionCalculatorTests
{
    [Test]
    public void CalculateEndDate_Month_Adds30Days()
    {
        var start = new DateTime(2025, 1, 1);
        var end = SubscriptionCalculator.CalculateEndDate(SubscriptionTypes.Month, start);
        Assert.That(end, Is.EqualTo(new DateTime(2025, 1, 31)));
    }

    [Test]
    public void CalculateEndDate_Quarter_Adds90Days()
    {
        var start = new DateTime(2025, 1, 1);
        var end = SubscriptionCalculator.CalculateEndDate(SubscriptionTypes.Quarter, start);
        Assert.That(end, Is.EqualTo(new DateTime(2025, 4, 1)));
    }

    [Test]
    public void CalculateEndDate_Year_Adds365Days()
    {
        var start = new DateTime(2025, 1, 1);
        var end = SubscriptionCalculator.CalculateEndDate(SubscriptionTypes.Year, start);
        Assert.That(end, Is.EqualTo(new DateTime(2026, 1, 1)));
    }

    [Test]
    public void IsActive_ReturnsTrueForValidSubscription()
    {
        var sub = new Subscription
        {
            Status = SubscriptionStatuses.Active,
            StartDate = DateTime.Today.AddDays(-5),
            EndDate = DateTime.Today.AddDays(10)
        };
        Assert.That(SubscriptionCalculator.IsActive(sub), Is.True);
    }

    [Test]
    public void IsActive_ReturnsFalseWhenExpired()
    {
        var sub = new Subscription
        {
            Status = SubscriptionStatuses.Active,
            StartDate = DateTime.Today.AddDays(-40),
            EndDate = DateTime.Today.AddDays(-1)
        };
        Assert.That(SubscriptionCalculator.IsActive(sub), Is.False);
    }

    [Test]
    public void IsActive_ReturnsFalseWhenStatusExpired()
    {
        var sub = new Subscription
        {
            Status = SubscriptionStatuses.Expired,
            StartDate = DateTime.Today.AddDays(-5),
            EndDate = DateTime.Today.AddDays(10)
        };
        Assert.That(SubscriptionCalculator.IsActive(sub), Is.False);
    }

    [Test]
    public void IsExpiringSoon_Within7Days()
    {
        var sub = new Subscription
        {
            Status = SubscriptionStatuses.Active,
            StartDate = DateTime.Today.AddDays(-23),
            EndDate = DateTime.Today.AddDays(5)
        };
        Assert.That(SubscriptionCalculator.IsExpiringSoon(sub), Is.True);
    }

    [Test]
    public void IsExpiringSoon_FalseWhenMoreThan7Days()
    {
        var sub = new Subscription
        {
            Status = SubscriptionStatuses.Active,
            StartDate = DateTime.Today.AddDays(-10),
            EndDate = DateTime.Today.AddDays(20)
        };
        Assert.That(SubscriptionCalculator.IsExpiringSoon(sub), Is.False);
    }

    [Test]
    public void DaysUntilExpiry_CalculatesCorrectly()
    {
        var sub = new Subscription { EndDate = DateTime.Today.AddDays(3) };
        Assert.That(SubscriptionCalculator.DaysUntilExpiry(sub), Is.EqualTo(3));
    }

    [Test]
    public void GetPrice_ReturnsExpectedValues()
    {
        Assert.That(SubscriptionTypes.GetPrice(SubscriptionTypes.Month), Is.EqualTo(2500m));
        Assert.That(SubscriptionTypes.GetPrice(SubscriptionTypes.Quarter), Is.EqualTo(6500m));
        Assert.That(SubscriptionTypes.GetPrice(SubscriptionTypes.Year), Is.EqualTo(22000m));
    }
}
