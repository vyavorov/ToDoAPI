using Microsoft.EntityFrameworkCore;
using System;
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
        public async Task<bool> CheckEmails(string userEmail, string invitedUserEmail)
        {

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            User? invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitedUserEmail);
            if (user == null) {
                throw new Exception("Inviting user does not exist.");
            }
            if (invitedUser == null)
            {
                throw new Exception("Invited user does not exist");
            }

            invitedUser.FamilyVerificationToken = Guid.NewGuid();
            invitedUser.FamilyVerificationTokenExpiration = DateTime.UtcNow.AddHours(24);

            return true;



            //family.Users.Add(user);
            //family.Users.Add(invitedUser);
            //user.FamilyId = family.Id;
           // await this._context.Families.AddAsync(family);
            //await this._context.SaveChangesAsync();
        }

        public async Task<bool> VerifyEmailAsync(Guid token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FamilyVerificationToken == token);
            if (user == null || user.EmailConfirmed || user.FamilyVerificationTokenExpiration < DateTime.UtcNow)
            {
                return false; // Email verification failed
            }

            // Verify the email
            user.FamilyConfirmed = true;
            user.FamilyVerificationToken = null; // Clear the verification token after successful verification

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return true; // Email verification succeeded
        }
    }
}
