using Application.Features.Plans.Queries.GetAllPlans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlansController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlansController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlans(CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetAllPlansQuery(), cancellationToken));
        }
    }
}
