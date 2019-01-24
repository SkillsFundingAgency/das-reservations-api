﻿using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
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
            try
            {
                var response = await _mediator.Send(new GetAccountReservationsQuery {AccountId = accountId});
                return Ok(response.Reservations);
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

        [HttpPost]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            try
            {
                var response = await _mediator.Send(new CreateAccountReservationCommand { Reservation = new Domain.Reservations.Reservation
                {
                    AccountId = reservation.AccountId,
                    StartDate = reservation.StartDate
                }});
                return Created($"api/accounts/{response.Reservation.AccountId}/{ControllerContext.ActionDescriptor.ControllerName}/{response.Reservation.Id}",response.Reservation);
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