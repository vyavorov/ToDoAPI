using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services;

public class PasswordHashService : IPasswordHashService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    public bool VerifyPassword(string providedPassword, string storedPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, storedPasswordHash);
    }
}
