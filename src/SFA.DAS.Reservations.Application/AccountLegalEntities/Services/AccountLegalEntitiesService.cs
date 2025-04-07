using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Services
{
    public class AccountLegalEntitiesService(
        IAccountLegalEntitiesRepository repository,
        IOptions<ReservationsConfiguration> configuration)
        : IAccountLegalEntitiesService
    {
        private readonly ReservationsConfiguration _configuration = configuration.Value;

        public async Task<IList<AccountLegalEntity>> GetAccountLegalEntities(long accountId)
        {
            var results = await repository.GetByAccountId(accountId);

            return results.Select(MapAccountLegalEntity).ToList();
        }

        public async Task<AccountLegalEntity> GetAccountLegalEntity(long accountLegalEntityId)
        {
            var entity = await repository.Get(accountLegalEntityId);
            return MapAccountLegalEntity(entity);
        }

        private AccountLegalEntity MapAccountLegalEntity(Domain.Entities.AccountLegalEntity accountLegalEntity)
        {
            var account = new Domain.Account.Account(accountLegalEntity.Account, _configuration.MaxNumberOfReservations);
            
            var mapAccountLegalEntity = new AccountLegalEntity(
                accountLegalEntity.Id, 
                accountLegalEntity.AccountId,
                accountLegalEntity.AccountLegalEntityName, 
                accountLegalEntity.LegalEntityId,
                accountLegalEntity.AccountLegalEntityId,                
                accountLegalEntity.AgreementSigned ,
                accountLegalEntity.Account.IsLevy
                );
            return mapAccountLegalEntity;
        }
    }
}