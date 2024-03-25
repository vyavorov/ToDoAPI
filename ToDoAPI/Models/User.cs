using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoAPI.Models;

public class User
{
    [Key]
    [Required]
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    public ICollection<Todo> Todos { get; set; }  = new List<Todo>();

    public bool EmailConfirmed { get; set; } = false;

    public Guid? VerificationToken { get; set; }

    public DateTime? VerificationTokenExpiration { get; set; }

    public Family? Family { get; set; }

    [ForeignKey(nameof(Family))]
    public Guid FamilyId { get; set; }

}
