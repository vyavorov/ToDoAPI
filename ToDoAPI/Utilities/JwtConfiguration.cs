namespace ToDoAPI.Utilities;

public static class JwtConfiguration
{
    public static string Issuer => "TodoApp";
    public static string Audience => "TodoApp";
    public static string SecretKey => JwtTokenSettings.GenerateRandomSecretKey();
}
