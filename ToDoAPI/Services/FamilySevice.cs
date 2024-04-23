using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services
{
    public class FamilySevice : IFamilyService
    {
        private readonly AppDbContext _context;

        public FamilySevice(AppDbContext appDbContext)
        {
            this._context = appDbContext;
        }
        public async Task CreateFamily(string userEmail, string familyName, string invitedUserEmail)
        {
            Family family = new Family()
            {
                Name = familyName
            };
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            User? invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitedUserEmail);
            if (user == null) {
                throw new Exception("User does not exist.");
            }
            if (invitedUser == null)
            {
                throw new Exception("Invited user does not exist");
            }
            family.Users.Add(user);
            //family.Users.Add(invitedUser);
            //user.FamilyId = family.Id;
            await this._context.Families.AddAsync(family);
            await this._context.SaveChangesAsync();
        }
    }
}
