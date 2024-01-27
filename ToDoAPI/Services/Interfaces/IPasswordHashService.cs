namespace ToDoAPI.Services.Interfaces;

public interface IPasswordHashService
{
    public string HashPassword(string password);

    public bool VerifyPassword(string providedPassword, string storedPasswordHash);
}
