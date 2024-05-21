using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces
{
    public interface IFamilyService
    {
        Task<bool> CheckEmails(string userEmail, string invitedUserEmail);

        Task<bool> VerifyEmailAsync(Guid token);

    }
}
