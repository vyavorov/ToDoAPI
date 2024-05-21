using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.DTOs;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHashService _passwordHashService;

    public AccountService(AppDbContext _dbContext, IPasswordHashService passwordHashService)
    {
        this._dbContext = _dbContext;
        _passwordHashService = passwordHashService;
    }

    public bool CheckIfEmailAndPasswordAreCorrect(string email, string password)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            bool isPasswordCorrect = _passwordHashService.VerifyPassword(password, user.PasswordHash);
            if (isPasswordCorrect)
            {
                return true;
            }
        }
        return false;

    }

    public async Task<bool> CheckIfEmailInDbAsync(UserDto user)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email == user.Email);
    }

    public Task<bool> CheckIfEmailIsValid(UserDto user)
    {
        return Task.FromResult(!string.IsNullOrEmpty(user.Email));
    }

    public async Task RegisterAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Guid> GetUserIdByEmail(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            return user.Id;
        }
        throw new Exception("There's not a user with this email");
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null)
        {
            return user;
        }
        throw new Exception("There's not a user with this email");
    }

    public async Task ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == changePasswordDto.Email);
        if (user == null)
        {
            throw new Exception("Email not exist in the database");
        }
        var newHashedPassword = _passwordHashService.HashPassword(changePasswordDto.NewPassword);
        user.PasswordHash = newHashedPassword;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsEmailConfirmed(string email)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new Exception("Email not exist in the database");
        }
        return user.EmailConfirmed;
    }

    public async Task<bool> VerifyEmailAsync(Guid token)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
        if (user == null || user.EmailConfirmed || user.VerificationTokenExpiration < DateTime.UtcNow)
        {
            return false; // Email verification failed
        }

        // Verify the email
        user.EmailConfirmed = true;
        user.VerificationToken = null; // Clear the verification token after successful verification

        // Save the changes to the database
        await _dbContext.SaveChangesAsync();

        return true; // Email verification succeeded
    }

}
