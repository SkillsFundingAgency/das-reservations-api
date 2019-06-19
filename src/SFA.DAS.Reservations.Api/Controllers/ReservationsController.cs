using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Exceptions;


namespace SFA.DAS.Reservations.Api.Controllers
{
    
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ILogger<ReservationsController> _logger;
        private readonly IMediator _mediator;

        public ReservationsController(
            ILogger<ReservationsController> logger,
            IMediator mediator)
        {
            _logger = logger;
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
                    AccountLegalEntityName = reservation.AccountLegalEntityName,
                    CreatedDate = DateTime.UtcNow
                });

                if (response.Rule != null)
                {
                    var modelStateDictionary = new ModelStateDictionary();
                    var errorMessage = $"{response.Rule.RuleTypeText} for {response.Rule.RestrictionText}";
                    modelStateDictionary.AddModelError(response.Rule.Id.ToString(),errorMessage);
                    _logger.LogWarning($"Rule Id: [{response.Rule.Id.ToString()}], error: [{errorMessage}]");
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
        public async Task<IActionResult> Validate(Guid id, string courseCode, DateTime startDate)
        {
            try
            {
                var request = new ValidateReservationQuery
                {
                    ReservationId = id,
                    CourseCode = courseCode,
                    StartDate = startDate
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

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(410)]
        [Route("api/[controller]/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteReservationCommand {ReservationId = id});
                return NoContent();
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException);
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = argumentException.Message,
                    Params = argumentException.ParamName
                });
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning(exception, exception.Message);
                return BadRequest();
            }
            catch (EntityNotFoundException<Domain.Entities.Reservation> notFoundException)
            {
                _logger.LogWarning(notFoundException, notFoundException.Message);
                return StatusCode(410);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(400)]
        [Route("/api/accounts/{accountLegalEntityId}/bulk-create")]
        public async Task<IActionResult> BulkCreate(int count)
        {
            throw new NotImplementedException();
        }
    }
}