using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models;

public class Family
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}
