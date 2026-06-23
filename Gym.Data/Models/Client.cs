using System.ComponentModel.DataAnnotations;

namespace Gym.Data.Models;

public class Client
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string LastName { get; set; } = string.Empty;

    [Required, MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string Phone { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public DateTime RegistrationDate { get; set; }

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Visit> Visits { get; set; } = new List<Visit>();

    public string FullName => $"{LastName} {FirstName}";
}
