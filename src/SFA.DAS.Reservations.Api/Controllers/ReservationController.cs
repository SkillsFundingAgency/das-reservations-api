using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/account/{accountId}/[controller]/")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAll(long accountId)
        {
            var response = await _mediator.Send(new GetAccountReservationsQuery{AccountId = accountId});

            return Ok(JsonConvert.SerializeObject(response));
        }

        [HttpPost]
        public async Task<IActionResult> Create(DateTime activeFrom)
        {
            return Accepted();
        }
    }
}
