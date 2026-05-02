using Application.Common;
using Application.Features.StudentLessons.Commands.CreateStudentDiscussion;
using Application.Features.StudentLessons.Commands.CreateStudentDiscussionReply;
using Application.Features.StudentLessons.Commands.DeleteStudentDiscussion;
using Application.Features.StudentLessons.Commands.DeleteStudentDiscussionReply;
using Application.Features.StudentLessons.Commands.UpdateStudentDiscussion;
using Application.Features.StudentLessons.Commands.UpdateStudentDiscussionReply;
using Application.Features.StudentLessons.Commands.UpdateStudentLessonProgress;
using Application.Features.StudentLessons.Queries.GetStudentDiscussions;
using Application.Features.StudentLessons.Queries.GetStudentLessonItem;
using Application.Features.StudentLessons.Queries.GetStudentLessonProgress;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/student/courses/{courseId}/items/{itemId}/lesson")]
    [ApiController]
    [Authorize]
    public class StudentLessonsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudentLessonsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("progress")]
        public async Task<IActionResult> GetStudentLessonProgress([FromRoute] int courseId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentLessonProgressQuery(courseId, itemId), cancellationToken);
            return result.Match(
                progress => Ok(progress),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("progress")]
        public async Task<IActionResult> UpdateStudentLessonProgress([FromRoute] int courseId, [FromRoute] int itemId, [FromBody] UpdateStudentLessonProgressCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ItemId = itemId }, cancellationToken);
            return result.Match(
                progress => Ok(progress),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet]
        public async Task<IActionResult> GetStudentLessonItem([FromRoute] int courseId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentLessonItemQuery(courseId, itemId), cancellationToken);
            return result.Match(
                lessonItem => Ok(lessonItem),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpGet("discussions")]
        public async Task<IActionResult> GetStudentDiscussions([FromRoute] int courseId, [FromRoute] int itemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentDiscussionsQuery(courseId, itemId), cancellationToken);
            return result.Match(
                discussions => Ok(discussions),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("discussions")]
        public async Task<IActionResult> CreateStudentDiscussion([FromRoute] int courseId, [FromRoute] int itemId, [FromBody] CreateStudentDiscussionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ItemId = itemId }, cancellationToken);
            return result.Match(
               discussion => Ok(discussion),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
           );
        }


        [HttpPut("discussions/{discussionId}")]
        public async Task<IActionResult> UpdateStudentDiscussion([FromRoute] int courseId, [FromRoute] int itemId, [FromRoute] int discussionId, [FromBody] UpdateStudentDiscussionCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ItemId = itemId, DiscussionId = discussionId }, cancellationToken);
            return result.Match(
               discussion => Ok(discussion),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("discussions/{discussionId}")]
        public async Task<IActionResult> DeleteStudentDiscussion([FromRoute] int courseId, [FromRoute] int itemId, [FromRoute] int discussionId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteStudentDiscussionCommand(courseId, itemId, discussionId), cancellationToken);
            return result.Match(
               discussion => Ok(discussion),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpPost("discussions/{discussionId}/replies")]
        public async Task<IActionResult> CreateStudentDiscussionReply([FromRoute] int courseId, [FromRoute] int itemId, [FromRoute] int discussionId, [FromBody] CreateStudentDiscussionReplyCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ItemId = itemId, DiscussionId = discussionId }, cancellationToken);
            return result.Match(
               discussionReply => Ok(discussionReply),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
           );
        }


        [HttpPut("discussions/{discussionId}/replies/{replyId}")]
        public async Task<IActionResult> UpdateStudentDiscussionReply([FromRoute] int courseId, [FromRoute] int itemId, [FromRoute] int discussionId, [FromRoute] int replyId, [FromBody] UpdateStudentDiscussionReplyCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with { CourseId = courseId, ItemId = itemId, DiscussionId = discussionId, ReplyId = replyId }, cancellationToken);
            return result.Match(
               discussionReply => Ok(discussionReply),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }


        [HttpDelete("discussions/{discussionId}/replies/{replyId}")]
        public async Task<IActionResult> DeleteStudentDiscussionReply([FromRoute] int courseId, [FromRoute] int itemId, [FromRoute] int discussionId, [FromRoute] int replyId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteStudentDiscussionReplyCommand(courseId, itemId, discussionId, replyId), cancellationToken);
            return result.Match(
               discussionReply => Ok(discussionReply),
                error => StatusCode((int)error.HttpStatusCode, new ErrorDto { Error = error.Message })
            );
        }
    }
}