using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Application.Rules.Queries;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> All()
        {
            var response = await _mediator.Send(new GetRulesQuery());

            return Ok(response.Rules);
        }
    }
}