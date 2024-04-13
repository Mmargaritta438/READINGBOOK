namespace AuthAi.Configuration
{
    public class JwtSettings
    {
        public const string Jwt = "JWT";
        public string Secret { get; set; } = string.Empty;
        public string TokenValidityInMinutes { get; set; } = string.Empty;
    }
}
