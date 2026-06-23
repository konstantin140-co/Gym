using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Models;

public class Visit
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public int SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;

    public DateTime DateTime { get; set; }

    [Required, MaxLength(40)]
    public string ActivityType { get; set; } = string.Empty;
}
