using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
                AttachUserToContext(context, token);
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
