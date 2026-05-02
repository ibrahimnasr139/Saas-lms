using Application.Common;
using Application.Constants;
using Application.Features.ModuleItems.Commands.CreateModuleItem;
using Application.Features.ModuleItems.Commands.DeleteModuleItem;
using Application.Features.ModuleItems.Commands.ReorderModuleItem;
using Application.Features.ModuleItems.Commands.UpdateSettings;
using Application.Features.ModuleItems.Queries.GetAll;
using Application.Features.ModuleItems.Queries.GetItem;
using Application.Features.ModuleItems.Queries.GetSettings;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/tenant/courses/{courseId}/modules/{moduleId}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = AuthConstants.ApiScheme)]
    public class ItemsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ItemsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateModuleItem(int courseId, int moduleId, [FromBody] CreateModuleItemCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ModuleId = moduleId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Created(string.Empty, success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpPatch("{itemId}/settings")]
        public async Task<IActionResult> UpdateSettings(int courseId, int moduleId, int itemId, [FromBody] UpdateSettingsCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ModuleId = moduleId, ItemId = itemId }, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpDelete("{itemId}")]
        public async Task<IActionResult> DeleteModuleItem([FromRoute] DeleteModuleItemCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetModuleItem([FromRoute] GetItemQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet("{itemId}/settings")]
        public async Task<IActionResult> GetSettings([FromRoute] GetSettingsQuery query, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllItems([FromRoute] GetAllItemsQuery query, [FromQuery] ModuleItemType? type, CancellationToken cancellationToken)
        {
            query = query with { Type = type };
            var result = await _mediator.Send(query, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(success),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }


        [HttpPost("reorder")]
        public async Task<IActionResult> ReorderItems(int courseId, int moduleId, [FromBody] List<OrderDto> items, CancellationToken cancellationToken)
        {
            var command = new ReorderModuleItemCommand(courseId, moduleId, items);
            var result = await _mediator.Send(command, cancellationToken);
            return result.Match<IActionResult>(
                success => Ok(),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message }));
        }
    }
}
