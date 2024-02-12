using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using ToDoAPI.Utilities;

namespace ToDoAPI.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    AttachUserToContext(context, token);
                }
                catch (SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token has expired");
                    return;
                }
                catch (SecurityTokenException)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var secretKey = _configuration.GetValue<string>("Jwt:SecretKey");
                var email = JwtUtility.ValidateToken(token, secretKey);

                // Attach the user to the context
                context.Items["User"] = email;
            }
            catch
            {
                // Do nothing if the token is invalid
            }
        }
    }
}
