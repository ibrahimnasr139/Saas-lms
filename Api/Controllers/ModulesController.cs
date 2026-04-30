using Application.Common;
using Application.Constants;
using Application.Features.Modules.Commands.CreateModule;
using Application.Features.Modules.Commands.DeleteModule;
using Application.Features.Modules.Commands.UpdateModule;
using Application.Features.Modules.Queries.GetAllModules;
using Application.Features.Modules.Queries.GetModuleById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class ModulesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ModulesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateModule([FromRoute] int courseId, CreateModuleCommand command, CancellationToken cancellationToken)
        {
            command = command with { CourseId = courseId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(
                success => Created(string.Empty, success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpPut("{moduleId}")]
        public async Task<IActionResult> UpdateModule([FromRoute] int courseId, [FromRoute] int moduleId, UpdateModuleCommand command, CancellationToken cancellationToken)
        {
            command = command with { CourseId = courseId, ModuleId = moduleId };
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpDelete("{moduleId}")]
        public async Task<IActionResult> DeleteModule([FromRoute] int courseId, [FromRoute] int moduleId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteModuleCommand(courseId, moduleId), cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllModules([FromRoute] GetAllModulesQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("{moduleId}")]
        public async Task<IActionResult> GetModuleById([FromRoute] GetModuleByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}
