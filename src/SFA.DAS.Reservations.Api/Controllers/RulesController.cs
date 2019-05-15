using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Api.Models;
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

            return Ok(response);
        }

        [Route("available-dates")]
        public async Task<IActionResult> GetAvailableDates(long accountLegalEntityId = 0)
        {
            //todo: wrap in try catch for potential not found exception
            var response =
                await _mediator.Send(new GetAvailableDatesQuery {AccountLegalEntityId = accountLegalEntityId});

            return Ok(response);
        }

        [Route("account/{accountId}")]
        public async Task<IActionResult> Account(long accountId)
        {
            try
            {
                var response = await _mediator.Send(new GetAccountRulesQuery { AccountId = accountId });

                return Ok(response);
            }
            catch (ArgumentException e)
            {
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = e.ParamName
                });
            }
            
        }
    }
}