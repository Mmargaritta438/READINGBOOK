using AuthAi.Interfaces;
using System.Text.RegularExpressions;

namespace AuthAi.Models.Authentication
{
    public class ChangePasswordModel : IModelValidation
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
        public string? RepeatNewPassword { get; set; }
        public string IsValid()
        {
            if (string.IsNullOrWhiteSpace(this.Email)) return "User Email is required";
            const string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"; //from MSDN
            if (!Regex.IsMatch(this.Email, pattern, RegexOptions.IgnoreCase)) return "Bad email";

            if (string.IsNullOrWhiteSpace(this.Password)) return "Password is required";
            if (this.Password.Length < 6 || this.Password.Length > 64) return "The password length must be between 6 and 64 characters";

            if (string.IsNullOrWhiteSpace(this.NewPassword)) return "Password is required";
            if (this.Password.Length < 6 || this.NewPassword.Length > 64) return "The password length must be between 6 and 64 characters";

            if (string.IsNullOrWhiteSpace(this.RepeatNewPassword)) return "Password is required";
            if (this.Password.Length < 6 || this.RepeatNewPassword.Length > 64) return "The password length must be between 6 and 64 characters";

            return string.Empty;
        }
    }
}
