using Application.Common;
using Application.Constants;
using Application.Features.Files.Commands.CallBack;
using Application.Features.Files.Commands.CreateUpload;
using Application.Features.Files.Commands.UploadFile;
using Application.Features.Files.Commands.VideoStatus;
using Application.Features.Files.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileCommand uploadFileCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(uploadFileCommand, cancellationToken);

            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("callback")]
        public async Task<IActionResult> CallBack([FromBody] CallBackCommand callBackCommand, CancellationToken cancellationToken)
        {
            await _mediator.Send(callBackCommand, cancellationToken);
            return NoContent();
        }


        [HttpPost("create-upload")]
        public async Task<IActionResult> CreateUpload([FromBody] CreateUploadCommand createUploadCommand, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(createUploadCommand, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPut("video-status/{id}")]
        public async Task<IActionResult> UpdateVideoStatus([FromRoute] string id, [FromBody] VideoStatusRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new VideoStatusCommand(id, request.Status, request.Size), cancellationToken);
            return NoContent();
        }
    }
}