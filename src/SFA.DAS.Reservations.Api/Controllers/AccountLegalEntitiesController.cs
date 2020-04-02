using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class AccountLegalEntitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountLegalEntitiesController> _logger;

        public AccountLegalEntitiesController(
            IMediator mediator,
            ILogger<AccountLegalEntitiesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpGet]
        [Route("/api/{accountId}/[controller]")]
        public async Task<IActionResult> GetByAccountId(long accountId)
        {
            try
            {
                var response = await _mediator.Send(new GetAccountLegalEntitiesQuery { AccountId = accountId });
                return Ok(response.AccountLegalEntities);
            }
            catch (ArgumentException e)
            {
                _logger.LogDebug($"Handled argument exception, Message:[{e.Message}], Params:[{e.ParamName}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = e.ParamName
                });
            }
        }

        [HttpGet]
        [Route("{legalEntityId}")]
        public async Task<IActionResult> GetByAccountLegalEntityId(long legalEntityId)
        {
            try
            {
                var response = await _mediator.Send(new GetAccountLegalEntityQuery { Id = legalEntityId });
                
                return Ok(response.LegalEntity);
            }
            catch (ArgumentException e)
            {
                _logger.LogDebug($"Handled argument exception, Message:[{e.Message}], Params:[{e.ParamName}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = e.ParamName
                });
            }
        }

        [HttpGet]
        [Route("/api/accounts/{accountId}/status")]
        public async Task<IActionResult> GetAccountReservationStatus(long accountId, [FromQuery]long? transferSenderId)
        {
            try
            {
                var response = await _mediator.Send(new GetAccountReservationStatusQuery
                {
                    AccountId = accountId,
                    TransferSenderAccountId = transferSenderId
                });
                var model = new AccountReservationStatus(response);
                return Ok(model);
            }
            catch (ArgumentException e)
            {
                _logger.LogDebug($"Handled argument exception, Message:[{e.Message}], Params:[{e.ParamName}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = e.ParamName
                });
            }
            catch (EntityNotFoundException<AccountLegalEntity> e)
            {
                _logger.LogDebug($"Handled EntityNotFoundException, Message:[{e.Message}]");
                return NotFound();
            }
        }

        [HttpGet]
        [Route("provider/{providerId}")]
        public async Task<IActionResult> GetByProviderId(uint providerId)
        {
            try
            {
                var response = await _mediator.Send(new GetAccountLegalEntitiesForProviderQuery { ProviderId = providerId });
                
                return Ok(response.ProviderPermissions);
            }
            catch (ArgumentException e)
            {
                _logger.LogDebug($"Handled argument exception, Message:[{e.Message}], Params:[{e.ParamName}]");
                return BadRequest(new ArgumentErrorViewModel
                {
                    Message = e.Message,
                    Params = e.ParamName
                });
            }
        }
    }
}