using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services;

public class PasswordHashService : IPasswordHashService
{
    private const int WorkFactor = 12;
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(WorkFactor));
    }

    public bool VerifyPassword(string providedPassword, string storedPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, storedPasswordHash);
    }
}
