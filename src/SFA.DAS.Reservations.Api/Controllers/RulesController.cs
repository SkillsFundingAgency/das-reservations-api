using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> All()
        {
            var response = await mediator.Send(new GetRulesQuery());

            return Ok(response);
        }

        [HttpGet]
        [Route("available-dates/{accountLegalEntityId}")]
        public async Task<IActionResult> GetAvailableDates(long accountLegalEntityId = 0)
        {
            try
            {
                var response =
                    await mediator.Send(new GetAvailableDatesQuery {AccountLegalEntityId = accountLegalEntityId});

                return Ok(response);
            }
            catch(Exception e) when(
                e is ArgumentException || 
                e is EntityNotFoundException<AccountLegalEntity>)
            {
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = nameof(accountLegalEntityId)
                });
            }
        }

        [HttpGet]
        [Route("account/{accountId}")]
        public async Task<IActionResult> Account(long accountId)
        {
            try
            {
                var response = await mediator.Send(new GetAccountRulesQuery { AccountId = accountId });

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

        [Route("upcoming")]
        [HttpPost]
        public async Task<IActionResult> AcknowledgeRuleAsRead(CreateUserRuleAcknowledgementCommand command)
        {
            await mediator.Send(command);

            return Ok();
        }
    }
}