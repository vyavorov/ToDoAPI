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
            Family family = new Family()
            {
                Name = createFamilyRequestDto.FamilyName
            };
            try
            {
                await this.familyService.CheckEmails(createFamilyRequestDto.UserEmail, createFamilyRequestDto.InvitedUserEmail);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                var user = await accountService.GetUserByEmail(createFamilyRequestDto.InvitedUserEmail);
                var invitingUser = createFamilyRequestDto.UserEmail;
                var verificationLink = Url.Action("VerifyEmail", "Family", new { token = user.FamilyVerificationToken }, Request.Scheme);
                await _emailService.SendEmailAsync(user.Email, "You were invited to a family", $"You were invited to {invitingUser}'s family. Please confirm by clicking <a href=\"{verificationLink}\">here</a>.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }




            return Ok(createFamilyRequestDto.FamilyName);
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(Guid token)
        {
            var result = await familyService.VerifyEmailAsync(token);

            if (!result)
            {
                return BadRequest("Invalid or expired verification token.");
            }

            return Ok("Email verified successfully.");
        }
    }
}
