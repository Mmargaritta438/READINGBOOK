using AuthAi.Models.Authentication;

namespace AuthAi.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponse> RegisterAsync(RegisterModel registerModel);
        Task<AuthServiceResponse> LoginAsync(LoginModel loginModel);
        Task<AuthServiceResponse> ConfirmAsync(ConfirmModel confirmModel);
        Task<AuthServiceResponse> ForgotPasswordAsync(string email);
        Task<AuthServiceResponse> ResetPasswordAsync(ResetPasswordModel resetPasswordModel);
        Task<AuthServiceResponse> ChangePasswordAsync(ChangePasswordModel changePasswordModel);
    }
}
