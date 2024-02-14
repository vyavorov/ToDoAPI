using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.DTOs;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;
using ToDoAPI.Utilities;

namespace ToDoAPI.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IAccountService _accountService;
    private readonly IPasswordHashService _passwordService;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AccountController(AppDbContext _context, IAccountService _accountService, IPasswordHashService _passwordService, IConfiguration configuration, IEmailService emailService)
    {
        this._context = _context;
        this._accountService = _accountService;
        this._passwordService = _passwordService;
        _configuration = configuration;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        try
        {
            if (await _accountService.CheckIfEmailInDbAsync(userDto))
            {
                return BadRequest(new { message = "This email has already been registered." });
            }

            if (!await _accountService.CheckIfEmailIsValid(userDto))
            {
                return BadRequest(new { message = "Invalid email. Please provide a valid email address." });
            }

            string hashedPassword = _passwordService.HashPassword(userDto.Password);
            Guid emailVerificationToken = Guid.NewGuid();

            User user = new User
            {
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                VerificationToken = emailVerificationToken,
            };

            var verificationLink = Url.Action("VerifyEmail", "Account", new { token = emailVerificationToken }, Request.Scheme);
            await _emailService.SendEmailAsync(userDto.Email, "Verify your email", $"Please verify your email by clicking <a href=\"{verificationLink}\">here</a>.");

            await _accountService.RegisterAsync(user);

            return Ok("Registration successful. Please check your email to verify your account.");
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserDto userDto)
    {
        try
        {

            if (_accountService.CheckIfEmailAndPasswordAreCorrect(userDto.Email, userDto.Password))
            {
                var secretKey = _configuration.GetValue<string>("Jwt:SecretKey");
                var userId = await _accountService.GetUserIdByEmail(userDto.Email);
                var token = JwtUtility.GenerateToken(userDto.Email, userId, secretKey);

                return Ok(new { Token = token });
            }
            return Unauthorized(new { message = "Invalid username or password" });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("changePassword")]

    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            if (_accountService.CheckIfEmailAndPasswordAreCorrect(changePasswordDto.Email, changePasswordDto.Password))
            {
                await _accountService.ChangePassword(changePasswordDto);
                return Ok();
            }
            return Unauthorized(new { message = "Password is wrong for this account" });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(Guid token)
    {
        // Find the user by the verification token
        var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

        // Check if the user exists and the email is not already verified
        if (user == null || user.EmailConfirmed)
        {
            return BadRequest("Invalid or expired verification token.");
        }

        // Verify the email
        user.EmailConfirmed = true;
        user.VerificationToken = null; // Clear the verification token after successful verification

        // Save the changes to the database
        await _context.SaveChangesAsync();

        return Ok("Email verified successfully.");
    }
}
