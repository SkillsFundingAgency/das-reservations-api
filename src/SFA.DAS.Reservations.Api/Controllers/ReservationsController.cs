using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/accounts/{accountId}/[controller]/")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll(long accountId)
        {
            var response = await _mediator.Send(new GetAccountReservationsQuery{AccountId = accountId});

            return Ok(response.Reservations);
        }
    }
}
