using AuthAi.Interfaces;
using AuthAi.Models;
using AuthAi.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthAi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        public AuthenticationController(UserManager<ApplicationUser> userManager, IAuthService authService)
        {
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServerResponse>> RegisterAsync([FromBody] RegisterModel registerModel)
        {
            var checkModelResult = CheckModel(registerModel);
            if (checkModelResult != null) return checkModelResult;

            var authServiceResponse = await _authService.RegisterAsync(registerModel);

            return ActionResponse(authServiceResponse);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServerResponse>> LoginAsync([FromBody] LoginModel loginModel)
        {
            var checkModelResult = CheckModel(loginModel);
            if (checkModelResult != null) return checkModelResult;

            var authServiceResponse = await _authService.LoginAsync(loginModel);

            return ActionResponse(authServiceResponse);
        }

        [HttpPost("confirmEmail")]
        public async Task<ActionResult<ServerResponse>> ConfirmEmail([FromBody] ConfirmModel confirmModel)
        {
            var checkModelResult = CheckModel(confirmModel);
            if (checkModelResult != null) return checkModelResult;

            var authServiceResponse = await _authService.ConfirmAsync(confirmModel);

            return ActionResponse(authServiceResponse);
        }

        [HttpPost("forgotPassword")]
        public async Task<ActionResult<ServerResponse>> ForgotPasswordAsync([FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            var checkModelResult = CheckModel(forgotPasswordModel);
            if (checkModelResult != null) return checkModelResult;

            var authServiceResponse = await _authService.ForgotPasswordAsync(forgotPasswordModel.Email!);

            return ActionResponse(authServiceResponse);
        }

        [HttpPost("resetPassword")]
        public async Task<ActionResult<ServerResponse>> ResetPasswordAsync([FromBody] ResetPasswordModel resetPasswordModel)
        {
            var checkModelResult = CheckModel(resetPasswordModel);
            if (checkModelResult != null) return checkModelResult;

            var authServiceResponse = await _authService.ResetPasswordAsync(resetPasswordModel);

            return ActionResponse(authServiceResponse);
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<ActionResult<ServerResponse>> ChangePasswordAsync([FromBody] ChangePasswordModel changePasswordModel)
        {
            var checkModelResult = CheckModel(changePasswordModel);
            if (checkModelResult != null) return checkModelResult;

            var authServiceResponse = await _authService.ChangePasswordAsync(changePasswordModel);

            return ActionResponse(authServiceResponse);
        }

        [Authorize]
        [HttpDelete("deleteUser")]
        public async Task<ActionResult<ServerResponse>> DeleteUser()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return UserNotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return UserNotFound();

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded
                ? Ok(new ServerResponse(ResponseStatus.Success))
                : BadRequest(new ServerResponse(ResponseStatus.CantDeleteUser));
        }

        private ActionResult<ServerResponse>? CheckModel(IModelValidation model)
        {
            var result = model.IsValid();
            if (!string.IsNullOrWhiteSpace(result)) return BadRequest(new ServerResponse(ResponseStatus.BadModel, result));
            return null;
        }

        private ActionResult<ServerResponse> UserNotFound() => BadRequest(new ServerResponse(ResponseStatus.UserNotExist, "Can't find user"));

        private ActionResult<ServerResponse> ActionResponse(AuthServiceResponse authServiceResponse) => authServiceResponse.IsSuccess
                ? Ok(new ServerResponse(ResponseStatus.Success, authServiceResponse.Data))
                : BadRequest(new ServerResponse(authServiceResponse.Status, authServiceResponse.Data));
    }
}
