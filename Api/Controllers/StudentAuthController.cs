using Application.Common;
using Application.Features.StudentAuth.Commands.Login;
using Application.Features.StudentAuth.Commands.Logout;
using Application.Features.StudentAuth.Commands.ResendOtp;
using Application.Features.StudentAuth.Commands.Signup;
using Application.Features.StudentAuth.Commands.VerifyOtp;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student/auth")]
    [ApiController]
    public class StudentAuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentAuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(new { Message = "Success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(new { Message = " success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCodeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(new { Message = "success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ResendOtpCommand(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(new { Message = "success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new LogoutCommand(), cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(new { Message = "success" }),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}