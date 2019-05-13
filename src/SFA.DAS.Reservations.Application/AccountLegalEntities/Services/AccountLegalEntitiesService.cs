using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Services
{
    public class AccountLegalEntitiesService
    {
        private readonly IAccountLegalEntitiesRepository _repository;

        public AccountLegalEntitiesService(IAccountLegalEntitiesRepository repository)
        {
            _repository = repository;
        }

        public async Task<IList<AccountLegalEntity>> GetAccountLegalEntities(long expectedAccountId)
        {
            var results = await _repository.GetByAccountId(expectedAccountId);

            return results.Select(MapReservation).ToList();
        }

        private AccountLegalEntity MapReservation(Domain.Entities.AccountLegalEntity accountLegalEntity)
        {
            var mapAccountLegalEntity = new AccountLegalEntity(accountLegalEntity.Id, accountLegalEntity.AccountId,
                accountLegalEntity.AccountLegalEntityName, accountLegalEntity.LegalEntityId,
                accountLegalEntity.AccountLegalEntityId);
            return mapAccountLegalEntity;
        }
    }
}