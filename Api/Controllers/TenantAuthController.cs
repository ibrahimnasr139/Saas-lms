using Application.Common;
using Application.Constants;
using Application.Features.TenantAuth.Commands.ForgetPassword;
using Application.Features.TenantAuth.Commands.Login;
using Application.Features.TenantAuth.Commands.Logout;
using Application.Features.TenantAuth.Commands.Refresh;
using Application.Features.TenantAuth.Commands.ResendOtp;
using Application.Features.TenantAuth.Commands.ResetPassword;
using Application.Features.TenantAuth.Commands.Signup;
using Application.Features.TenantAuth.Commands.VerifyOtp;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/auth")]
    [ApiController]
    public class TenantAuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TenantAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupCommand signupCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(signupCommand, cancellationToken);
            return result.Match(
                success => Created(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(ResendOtpCommand resendOtpCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(resendOtpCommand, cancellationToken);
            return result.Match(
                success => Ok(new { Message = "Success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpCommand verifyOtpCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(verifyOtpCommand, cancellationToken);
            return result.Match(
                success => Ok(new { Message = "Success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(loginCommand, cancellationToken);
            return result.Match(
                loginDto => Ok(loginDto),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordCommand forgetPasswordCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(forgetPasswordCommand, cancellationToken);
            return result.Match(
                success => Ok(new { Message = "success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(resetPasswordCommand, cancellationToken);
            return result.Match(
                success => Ok(new { Message = "Password reset successfully" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var token = Request.Cookies[AuthConstants.RefreshToken];
            var refreshCommand = new RefreshCommand(string.IsNullOrEmpty(token) ? null : token.ToString());
            var result = await _mediator.Send(refreshCommand, cancellationToken);
            return result.Match(
                success => Ok(new { Message = "Success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var logoutCommand = new LogoutCommand();
            var result = await _mediator.Send(logoutCommand, cancellationToken);
            return result.Match(
                success => Ok(new { Message = "Success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}
