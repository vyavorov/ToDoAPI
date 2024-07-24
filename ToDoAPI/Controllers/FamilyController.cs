using Microsoft.AspNetCore.Mvc;
using ToDoAPI.DTOs;
using ToDoAPI.Models;
using ToDoAPI.Services;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("api/families")]
    public class FamilyController : Controller
    {
        private readonly IFamilyService familyService;
        private readonly IAccountService accountService;
        private readonly IEmailService _emailService;
        public FamilyController(IFamilyService familyService, IAccountService accountService, IEmailService emailService)
        {
            this.familyService = familyService;
            this.accountService = accountService;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<ActionResult<Family>> CreateFamily([FromBody] CreateFamilyRequestDto createFamilyRequestDto)
        {
            try
            {
                User invitedUser = await accountService.GetUserByEmail(createFamilyRequestDto.InvitedUserEmail);
                User invitingUser = await accountService.GetUserByEmail(createFamilyRequestDto.UserEmail);
                Family currentFamily = null;
                if (invitingUser.FamilyId != null)
                {
                    currentFamily = await familyService.GetFamilyById((Guid)invitingUser.FamilyId);
                }
                if (createFamilyRequestDto.FamilyName != currentFamily.Name)
                {
                    throw new Exception("Only the head of the family can invite users");
                }
                if (invitedUser == null)
                {
                    throw new Exception("Invited user does not exist");
                }
                if (invitedUser.FamilyId != null)
                {
                    throw new Exception("Invited user is already a part of a family.");
                }
                if (createFamilyRequestDto.UserEmail == createFamilyRequestDto.InvitedUserEmail)
                {
                    throw new Exception("You can not invite yourself");
                }

                Family family = await this.familyService.DoesFamilyExist(createFamilyRequestDto.FamilyName);
                if (family == null)
                {
                    family = new Family()
                    {
                        Name = createFamilyRequestDto.FamilyName
                    };
                }

                await this.familyService.CheckEmails(createFamilyRequestDto.UserEmail, createFamilyRequestDto.InvitedUserEmail);
                await this.familyService.AssignUserToFamily(family, createFamilyRequestDto.UserEmail, createFamilyRequestDto.InvitedUserEmail);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            try
            {
                var user = await accountService.GetUserByEmail(createFamilyRequestDto.InvitedUserEmail);
                var invitingUserEmail = createFamilyRequestDto.UserEmail;
                var invitingUser = await accountService.GetUserByEmail(invitingUserEmail);
                var familyId = invitingUser.FamilyId;
                var verificationLink = Url.Action("VerifyEmail", "Family", new { token = user.FamilyVerificationToken, familyId = familyId }, Request.Scheme);
                await _emailService.SendEmailAsync(user.Email, "You were invited to a family", $"You were invited to {invitingUserEmail}'s family. Please confirm by clicking <a href=\"{verificationLink}\">here</a>.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(createFamilyRequestDto.FamilyName);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(Guid token, Guid familyId)
        {
            var result = await familyService.VerifyEmailAsync(token, familyId);

            if (!result)
            {
                return BadRequest("Invalid or expired verification token.");
            }

            return Ok("You are now part of a family!");
        }
    }
}
