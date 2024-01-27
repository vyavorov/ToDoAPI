namespace ToDoAPI.Utilities;

public static class JwtTokenSettings
{
    public static string GenerateRandomSecretKey()
    {
        var random = new Random();
        var bytes = new byte[32];

        random.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
