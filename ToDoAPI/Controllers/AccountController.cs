using Microsoft.AspNetCore.Mvc;
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

    public AccountController(AppDbContext _context, IAccountService _accountService, IPasswordHashService _passwordService, IConfiguration configuration)
    {
        this._context = _context;
        this._accountService = _accountService;
        this._passwordService = _passwordService;
        _configuration = configuration;
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

            User user = new User
            {
                Email = userDto.Email,
                PasswordHash = hashedPassword
            };

            await _accountService.RegisterAsync(user);

            return Ok("Registration successful");
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
}
