using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models;

public class User
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public ICollection<Todo> Todos { get; set; }  = new List<Todo>();

}
