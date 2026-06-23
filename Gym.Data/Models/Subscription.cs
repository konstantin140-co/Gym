using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Models;

public class Subscription
{
    public int Id { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    [Required, MaxLength(30)]
    public string Type { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = string.Empty;

    public ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
