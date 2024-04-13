namespace AuthAi.Models
{
    public class AppMailOptions
    {
        public const string AppMail = "AppMail";
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SmtpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string MessageIdDomain { get; set; } = string.Empty;
    }
}
