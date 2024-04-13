using AuthAi.Configuration;
using AuthAi.Interfaces;
using AuthAi.Models.Authentication;
using AuthAi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace AuthAi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly DataContext _context;
        public AuthService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, IEmailService emailService, DataContext context)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _emailService = emailService;
            _context = context;
        }

        public async Task<AuthServiceResponse> RegisterAsync(RegisterModel registerModel)
        {
            if (registerModel.Email == registerModel.Password)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.EqualEmailAndPassword, Data = "Email and password must be different" };
            }
            var existingUser = await _userManager.FindByEmailAsync(registerModel.Email!.ToLower());
            if (existingUser is { EmailConfirmed: false })
            {
                await _userManager.DeleteAsync(existingUser);
                existingUser = null;
            }
            if (existingUser != null)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.UserExists, Data = "User with this email already registered" };
            }
            var user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerModel.Email.ToLower(),
                Email = registerModel.Email.ToLower()
            };

            var createdUser = await _userManager.CreateAsync(user, registerModel.Password!);
            if (!createdUser.Succeeded)
            {
                return new AuthServiceResponse
                {
                    IsSuccess = false,
                    Status = ResponseStatus.CantCreateUser,
                    Data = createdUser.Errors.Select(x => x.Description).FirstOrDefault() ?? "Can not register user"
                };
            }
            var systemUser = await _userManager.FindByEmailAsync(registerModel.Email.ToLower());

            var confirmationCode = await GenerateConfirmationCodeAsync(systemUser);

            await _emailService.SendEmailAsync(systemUser!.Email!, "Account confirmation code", confirmationCode.Code);

            return new AuthServiceResponse { IsSuccess = true, Status = ResponseStatus.Success, Data = "Check your email and confirm with confirmation code" };
        }

        public async Task<AuthServiceResponse> LoginAsync(LoginModel loginModel)
        {

            var user = await _userManager.FindByEmailAsync(loginModel.Email!);
            if (user is null)
            {
                return NoUser();
            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, loginModel.Password!);
            if (!userHasValidPassword)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.WrongPassword, Data = "User/password combination is wrong" };
            }
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!isEmailConfirmed)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.EmailNotConfirmed, Data = "Email is not confirmed" };
            }
            return CreateToken(user);
        }

        public async Task<AuthServiceResponse> ConfirmAsync(ConfirmModel confirmModel)
        {
            var user = await _userManager.FindByEmailAsync(confirmModel.Email!.ToLower());
            if (user is null)
            {
                return NoUser();
            }

            var code = await _context.ConfirmationCodes.Where(c => c.UserEmail == confirmModel.Email.ToLower() && c.Code == confirmModel.Code).FirstOrDefaultAsync();
            if (code is null)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.WrongCode, Data = "There is no such code for this user" };
            }

            if (code.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.CodeExpired, Data = "Code expired" };
            }

            _context.ConfirmationCodes.Remove(code);
            await _context.SaveChangesAsync();
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return new AuthServiceResponse { IsSuccess = true, Status = ResponseStatus.Success, Data = "Code confirmed" };
        }

        public async Task<AuthServiceResponse> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email.ToLower());
            if (user is null)
            {
                return NoUser();
            }

            var confirmationCode = await GenerateConfirmationCodeAsync(user);

            await _emailService.SendEmailAsync(email, "Reset password confirmation code", confirmationCode.Code);

            return new AuthServiceResponse { IsSuccess = true, Status = ResponseStatus.Success, Data = "Check your email and reset password with confirmation code" };
        }

        public async Task<AuthServiceResponse> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email!.ToLower());
            if (user is null)
            {
                return NoUser();
            }
            if (resetPasswordModel.Password != resetPasswordModel.RepeatPassword)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.WrongRepeatPassword, Data = "Password and repeat password are not equal" };
            }

            var code = await _context.ConfirmationCodes
                .Where(c => c.UserEmail == resetPasswordModel.Email.ToLower() && c.Code == resetPasswordModel.Code)
                .FirstOrDefaultAsync();

            if (code is null)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.WrongCode, Data = "There is no such code for this user" };
            }

            if (code.ExpiryDate < DateTime.UtcNow)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.CodeExpired, Data = "Code expired" };
            }

            _context.ConfirmationCodes.Remove(code);
            await _context.SaveChangesAsync();

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, resetPasswordModel.Password!);

            return new AuthServiceResponse { IsSuccess = true, Status = ResponseStatus.Success, Data = "Password changed" };
        }

        public async Task<AuthServiceResponse> ChangePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(changePasswordModel.Email!.ToLower());
            if (user is null)
            {
                return NoUser();
            }
            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, changePasswordModel.Password!);
            if (!userHasValidPassword)
            {
                return new AuthServiceResponse { IsSuccess = false, Status = ResponseStatus.WrongPassword, Data = "User/password combination is wrong" };
            }
            if (changePasswordModel.NewPassword != changePasswordModel.RepeatNewPassword)
            {
                return new AuthServiceResponse
                { IsSuccess = false, Status = ResponseStatus.WrongRepeatPassword, Data = "New password and repeat new password are not equal" };
            }

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, changePasswordModel.NewPassword!);

            return new AuthServiceResponse { IsSuccess = true, Status = ResponseStatus.Success, Data = "Password changed" };
        }

        private AuthServiceResponse CreateToken(ApplicationUser user)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            _ = int.TryParse(_jwtSettings.TokenValidityInMinutes, out int tokenValidityInMinutes);

            var authClaims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Name, user.UserName!),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddMinutes(tokenValidityInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            var result = new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };

            return new AuthServiceResponse { IsSuccess = true, Status = ResponseStatus.Success, Data = result };
        }

        private async Task<ConfirmationCode> GenerateConfirmationCodeAsync(ApplicationUser? systemUser)
        {
            var confirmationCode = new ConfirmationCode
            {
                UserEmail = systemUser!.Email!,
                Code = Random.Shared.Next(0, 999999).ToString("D6"),
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            };

            await _context.ConfirmationCodes.AddAsync(confirmationCode);

            //Remove old codes
            var expiredCodes = await _context.ConfirmationCodes.Where(c => c.ExpiryDate < DateTime.UtcNow).ToListAsync();
            _context.ConfirmationCodes.RemoveRange(expiredCodes);
            await _context.SaveChangesAsync();
            return confirmationCode;
        }
        private static AuthServiceResponse NoUser()
            => new() { IsSuccess = false, Status = ResponseStatus.UserNotExist, Data = "There is no registered user with this email" };
    }
}
