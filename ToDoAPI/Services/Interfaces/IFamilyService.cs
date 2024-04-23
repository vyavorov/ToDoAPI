using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces
{
    public interface IFamilyService
    {
        Task CreateFamily(string userEmail, string familyName, string invitedUserEmail);
    }
}
