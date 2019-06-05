using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AccountLegalEntitiesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountLegalEntitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("{accountId}")]
        public async Task<IActionResult> GetByAccountId(long accountId)
        {
            try
            {
                var response = await _mediator.Send(new GetAccountLegalEntitiesQuery { AccountId = accountId });
                return Ok(response.AccountLegalEntities);
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