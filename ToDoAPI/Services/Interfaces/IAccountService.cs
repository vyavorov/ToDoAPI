using ToDoAPI.DTOs;
using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces;

public interface IAccountService
{
    public Task RegisterAsync(User user);

    public Task<bool> CheckIfEmailInDbAsync(UserDto user);

    public Task<bool> CheckIfEmailIsValid(UserDto user);

    public bool CheckIfEmailAndPasswordAreCorrect(string email, string password);

    public Task<Guid> GetUserIdByEmail(string email);
    public Task<User> GetUserByEmail(string email);


    public Task ChangePassword(ChangePasswordDto changePasswordDto);

    public Task<bool> IsEmailConfirmed(string email);

    Task<bool> VerifyEmailAsync(Guid token);


}
