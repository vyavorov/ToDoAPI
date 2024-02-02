using Microsoft.Extensions.Configuration;

namespace ToDoAPI.Utilities;

public static class JwtTokenSettings
{
    public static string GetSecretKey(IConfiguration configuration)
    {
        return configuration.GetValue<string>("Jwt:SecretKey");
    }
}
