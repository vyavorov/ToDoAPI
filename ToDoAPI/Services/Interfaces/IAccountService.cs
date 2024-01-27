using ToDoAPI.DTOs;
using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces;

public interface IAccountService
{
    public Task RegisterAsync(User user);

    public Task<bool> CheckIfEmailInDbAsync(UserDto user);

    public Task<bool> CheckIfEmailIsValid(UserDto user);


}
