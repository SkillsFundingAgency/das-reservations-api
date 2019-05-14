using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Services
{
    public class AccountLegalEntitiesService : IAccountLegalEntitiesService
    {
        private readonly IAccountLegalEntitiesRepository _repository;
        private readonly IGlobalRulesService _globalRulesService;

        public AccountLegalEntitiesService(IAccountLegalEntitiesRepository repository,
            IGlobalRulesService globalRulesService)
        {
            _repository = repository;
            _globalRulesService = globalRulesService;
        }

        public async Task<IList<AccountLegalEntity>> GetAccountLegalEntities(long expectedAccountId)
        {
            var results = await _repository.GetByAccountId(expectedAccountId);

            return results.Select(MapReservation).ToList();
        }

        private AccountLegalEntity MapReservation(Domain.Entities.AccountLegalEntity accountLegalEntity)
        {
            var reservationLimit = _globalRulesService.GetReservationLimit();
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