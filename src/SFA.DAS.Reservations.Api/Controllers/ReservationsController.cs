using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;


namespace SFA.DAS.Reservations.Api.Controllers
{
    
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(400)]
        [Route("api/accounts/{accountId}/[controller]/")]
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
        [Route("api/accounts/{accountId}/[controller]/")]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            try
            {
                var response = await _mediator.Send(new CreateAccountReservationCommand
                {
                    Id = reservation.Id,
                    AccountId = reservation.AccountId,
                    StartDate = reservation.StartDate,
                    CourseId = reservation.CourseId,
                    ProviderId = reservation.ProviderId,
                    AccountLegalEntityId = reservation.AccountLegalEntityId,
                    AccountLegalEntityName = reservation.AccountLegalEntityName
                    
                });

                if (response.Rule != null)
                {
                    var modelStateDictionary = new ModelStateDictionary();
                    modelStateDictionary.AddModelError(response.Rule.Id.ToString(),$"{response.Rule.RuleTypeText} for {response.Rule.RestrictionText}");
                    return UnprocessableEntity(modelStateDictionary);
                }

                return Created($"api/{ControllerContext.ActionDescriptor.ControllerName}/{response.Reservation.Id}",response.Reservation);
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

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("api/[controller]/{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var response = await _mediator.Send(new GetReservationQuery
                {
                    Id = id
                });

                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response.Reservation);
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

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [Route("api/[controller]/validate/{id}")]
        public async Task<IActionResult> Validate(Guid id, string courseId, DateTime trainingStartDate)
        {
            try
            {
                var request = new ValidateReservationQuery
                {
                    ReservationId = id,
                    CourseId = courseId,
                    TrainingStartDate = trainingStartDate
                };

                var response = await _mediator.Send(request);

                if (response == null)
                {
                    return NotFound();
                }

                return Ok(response.Errors);
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