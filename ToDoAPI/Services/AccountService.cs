using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.DTOs;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _dbContext;

    public AccountService(AppDbContext _dbContext)
    {
        this._dbContext = _dbContext;
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
}
