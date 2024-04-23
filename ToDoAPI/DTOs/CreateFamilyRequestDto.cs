using ToDoAPI.Models;

namespace ToDoAPI.DTOs;

public class CreateFamilyRequestDto
{
    public string UserEmail { get; set; } = null!;
    public string FamilyName { get; set; } = null!;
    public string InvitedUserEmail { get; set; } = null!;
}
