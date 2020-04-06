﻿using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Api.Models.Reservation;


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

        [HttpGet]
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
                    CreatedDate = DateTime.UtcNow,
                    IsLevyAccount = reservation.IsLevyAccount,
                    TransferSenderAccountId = reservation.TransferSenderAccountId,
                    UserId = reservation.UserId
                });

                if (response.Rule != null)
                {
                    var modelStateDictionary = new ModelStateDictionary();
                    var errorMessage = $"{response.Rule.RuleTypeText} for {response.Rule.RestrictionText}";
                    modelStateDictionary.AddModelError(response.Rule.Id.ToString(),errorMessage);
                    _logger.LogWarning($"Rule Id: [{response.Rule.Id.ToString()}], error: [{errorMessage}]");
                    return UnprocessableEntity(modelStateDictionary);
                }

                if (!response.AgreementSigned)
                {
                    var modelStateDictionary = new ModelStateDictionary();
                    var errorMessage = $"None levy Account Id: {reservation.AccountId} has tried to make reservation with no agreement signed";
                    modelStateDictionary.AddModelError("ReservationFailure", errorMessage);
                    _logger.LogWarning($"None levy no agreement reservation creation failure, error: [{errorMessage}]");
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
        [Route("api/[controller]/search")]
        public async Task<IActionResult> Search(
            long providerId, string searchTerm, string selectedCourse, string selectedEmployer, string selectedStartDate, ushort pageNumber = 1, ushort pageItemCount = 10)
        {
            try
            {
                var response = await _mediator.Send(new FindAccountReservationsQuery
                {
                    ProviderId = providerId, 
                    SearchTerm = searchTerm,
                    PageNumber = pageNumber,
                    PageItemCount = pageItemCount,
                    SelectedFilters = new SelectedSearchFilters
                    {
                        CourseFilter = selectedCourse,
                        EmployerNameFilter = selectedEmployer,
                        StartDateFilter = selectedStartDate
                    }
                });

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

                return Ok(new ValidateReservationViewModel
                {
                    ValidationErrors = response.Errors
                        .Select(e => new ReservationValidationErrorViewModel
                        {
                            PropertyName = e.PropertyName,
                            Reason = e.Reason
                        })
                        .ToList()
                });
            }
            catch (ArgumentException e)
            {
                _logger.LogWarning(e, e.Message);
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = e.ParamName
                });
            }
            catch (EntityNotFoundException<Domain.Reservations.Reservation> e)
            {
                _logger.LogError(e, e.Message);
                return NotFound();
            }
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(410)]
        [Route("api/[controller]/{id}")]
        public async Task<IActionResult> Delete(Guid id, bool employerDeleted)
        {
            try
            {
                await _mediator.Send(new DeleteReservationCommand {ReservationId = id, EmployerDeleted = employerDeleted});
                return NoContent();
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogDebug($"Handled ArgumentException, Message:[{argumentException.Message}]");
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
        [ProducesResponseType(400)]
        [Route("/api/[controller]/accounts/{accountLegalEntityId}/bulk-create")]
        public async Task<IActionResult> BulkCreate([FromRoute]long accountLegalEntityId, [FromBody]BulkReservation bulkReservation)
        {
            try
            {
                var result = await _mediator.Send(new BulkCreateAccountReservationsCommand
                {
                    AccountLegalEntityId = accountLegalEntityId,
                    ReservationCount = bulkReservation.Count,
                    TransferSenderAccountId = bulkReservation.TransferSenderId
                });
                return Created("", result);
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogDebug($"Handled ArgumentException, Message:[{argumentException.Message}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = argumentException.Message,
                    Params = argumentException.ParamName
                });
            }
            catch (EntityNotFoundException<Domain.Entities.AccountLegalEntity> e)
            {
                _logger.LogDebug($"Handled EntityNotFoundException, Message:[{e.Message}]");
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [Route("api/[controller]/{id}/change")]
        public async Task<IActionResult> Change(Guid id, ChangeOfPartyRequest request)
        {
            try
            {
                var result = await _mediator.Send(new ChangeOfPartyCommand
                {
                    ReservationId = id,
                    AccountLegalEntityId = request.AccountLegalEntityId,
                    ProviderId = request.ProviderId
                });
                return Ok(new ChangeOfPartyResponse
                {
                    ReservationId = result.ReservationId
                });
            }
            catch (ArgumentException argumentException)
            {
                _logger.LogDebug($"Handled ArgumentException, Message:[{argumentException.Message}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = argumentException.Message,
                    Params = argumentException.ParamName
                });
            }
            catch (EntityNotFoundException<Domain.Entities.Reservation> notFoundException)
            {
                _logger.LogDebug(notFoundException, $"Handled EntityNotFoundException, Message:[{notFoundException.Message}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = notFoundException.Message
                });
            }
            catch (EntityNotFoundException<Domain.Entities.AccountLegalEntity> notFoundException)
            {
                _logger.LogDebug(notFoundException, $"Handled EntityNotFoundException, Message:[{notFoundException.Message}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = notFoundException.Message
                });
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _logger.LogDebug(invalidOperationException, $"Handled InvalidOperationException, Message:[{invalidOperationException.Message}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = invalidOperationException.Message
                });
            }
        }
    }
}