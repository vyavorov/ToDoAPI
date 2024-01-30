using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ToDoAPI.DTOs; // Make sure to include the namespace where your UserDto is defined

namespace ToDoAPI.Utilities
{
    public static class JwtUtility
    {
        private static readonly string SecretKey = JwtTokenSettings.GenerateRandomSecretKey();

        public static string GenerateToken(string email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                // Add additional claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                JwtConfiguration.Issuer, 
                JwtConfiguration.Audience, 
                claims,
                expires: DateTime.Now.AddMinutes(30), // Set token expiration time as needed
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
