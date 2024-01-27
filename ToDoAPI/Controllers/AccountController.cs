﻿using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Data;
using ToDoAPI.DTOs;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly IAccountService _accountService;
    private readonly IPasswordHashService _passwordService;

    public AccountController(AppDbContext _context, IAccountService _accountService, IPasswordHashService _passwordService)
    {
        this._context = _context;
        this._accountService = _accountService;
        this._passwordService = _passwordService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto userDto)
    {
        try
        {
            if (await _accountService.CheckIfEmailInDbAsync(userDto))
            {
                return BadRequest("This email has already been registered.");
            }

            if (!await _accountService.CheckIfEmailIsValid(userDto))
            {
                return BadRequest("Invalid email. Please provide a valid email address.");
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
}