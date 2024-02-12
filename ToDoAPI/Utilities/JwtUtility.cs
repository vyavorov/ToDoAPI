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
        public static string GenerateToken(string email, Guid userId, string sectetKey)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                // Add additional claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sectetKey));
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

        public static string ValidateToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = JwtConfiguration.Issuer,
                    ValidAudience = JwtConfiguration.Audience
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken?.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new SecurityTokenExpiredException("Token has expired", ex);
            }
            catch (SecurityTokenException ex)
            {
                throw new SecurityTokenException("Invalid token", ex);
            }
        }
    }
}
