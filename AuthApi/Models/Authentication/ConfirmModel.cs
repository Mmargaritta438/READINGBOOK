using AuthAi.Interfaces;
using System.Text.RegularExpressions;

namespace AuthAi.Models.Authentication
{
    public class ConfirmModel : IModelValidation
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string IsValid()
        {
            if (string.IsNullOrWhiteSpace(this.Email)) return "User Email is required";
            const string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$"; //from MSDN
            if (!Regex.IsMatch(this.Email, pattern, RegexOptions.IgnoreCase)) return "Bad email";

            if (string.IsNullOrWhiteSpace(this.Code)) return "Confirmation code is required";

            return string.Empty;
        }
    }
}
