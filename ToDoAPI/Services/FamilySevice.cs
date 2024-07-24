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

        public async Task AssignUserToFamily(Family family, string userEmail, string invitedUserEmail)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            User? invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitedUserEmail);

            invitedUser.FamilyVerificationToken = Guid.NewGuid();
            invitedUser.FamilyVerificationTokenExpiration = DateTime.UtcNow.AddHours(24);

            var isUserInFamily = family.Users.FirstOrDefault(u => u.Email == userEmail);
            
            if (invitedUser.FamilyId != null)
            {
                throw new Exception("Invited user already in family");
            }
            if (isUserInFamily == null)
            {
                family.Users.Add(user);
                user.FamilyId = family.Id;
                user.FamilyConfirmed = true;
            }
            family.Users.Add(invitedUser);
            var doesFamilyExist = await this._context.Families.FirstOrDefaultAsync(f =>  f.Id == family.Id);
            if (doesFamilyExist == null)
            {
                await this._context.Families.AddAsync(family);
            }
            await _context.SaveChangesAsync();
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
            return true;
        }

        public async Task<Family> DoesFamilyExist(string familyName)
        {
            Family? family = await _context.Families.Where(f => f.Name == familyName).FirstOrDefaultAsync();

            return family;
        }

        public async Task<Family> GetFamilyById(Guid familyId)
        {
            Family family = await _context.Families.FirstOrDefaultAsync(f => f.Id == familyId);
            return family;
        }

        public async Task<bool> VerifyEmailAsync(Guid token, Guid familyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FamilyVerificationToken == token);
            if (user == null || user.FamilyConfirmed || user.FamilyVerificationTokenExpiration < DateTime.UtcNow)
            {
                return false; // Email verification failed
            }

            // Verify the email
            user.FamilyConfirmed = true;
            user.FamilyVerificationToken = null; // Clear the verification token after successful verification
            user.FamilyId = familyId;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return true; // Email verification succeeded
        }
    }
}
