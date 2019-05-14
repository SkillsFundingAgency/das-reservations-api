﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Services
{
    public class AccountLegalEntitiesService : IAccountLegalEntitiesService
    {
        private readonly IAccountLegalEntitiesRepository _repository;
        private readonly ReservationsConfiguration _configuration;

        public AccountLegalEntitiesService(IAccountLegalEntitiesRepository repository,
            IOptions<ReservationsConfiguration> configuration)
        {
            _repository = repository;
            _configuration = configuration.Value;
        }

        public async Task<IList<AccountLegalEntity>> GetAccountLegalEntities(long accountId)
        {
            var results = await _repository.GetByAccountId(accountId);

            return results.Select(MapReservation).ToList();
        }

        private AccountLegalEntity MapReservation(Domain.Entities.AccountLegalEntity accountLegalEntity)
        {
            var reservationLimit = _configuration.MaxNumberOfReservations;

            if (accountLegalEntity.ReservationLimit.HasValue)
            {
                reservationLimit = accountLegalEntity.ReservationLimit.Value;
            }

            var mapAccountLegalEntity = new AccountLegalEntity(
                accountLegalEntity.Id, 
                accountLegalEntity.AccountId,
                accountLegalEntity.AccountLegalEntityName, 
                accountLegalEntity.LegalEntityId,
                accountLegalEntity.AccountLegalEntityId,
                reservationLimit);
            return mapAccountLegalEntity;
        }
    }
}