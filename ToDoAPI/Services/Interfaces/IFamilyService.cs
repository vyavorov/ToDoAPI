using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces
{
    public interface IFamilyService
    {
        Task<bool> CheckEmails(string userEmail, string invitedUserEmail);

        Task<bool> VerifyEmailAsync(Guid token, Guid familyId);

        Task<Family> DoesFamilyExist(string familyName);

        Task AssignUserToFamily(Family family, string userEmail, string invitedUserEmail);

        Task<Family> GetFamilyById(Guid familyId);

    }
}
