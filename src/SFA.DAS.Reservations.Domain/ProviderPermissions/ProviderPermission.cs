using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public class ProviderPermission
    {
        public ProviderPermission (Domain.Entities.ProviderPermission providerPermission)
        {
            AccountId = providerPermission.AccountId;
            AgreementSigned = providerPermission.AccountLegalEntity.AgreementSigned;
            UkPrn = (uint) providerPermission.UkPrn;
            AccountLegalEntityId = providerPermission.AccountLegalEntityId;
            AccountLegalEntityName = providerPermission.AccountLegalEntity.AccountLegalEntityName;
            AgreementType = providerPermission.AccountLegalEntity.AgreementType;
        }

        public long AccountId { get ; }
        public uint UkPrn { get ;  }
        public long AccountLegalEntityId { get ; }
        public bool AgreementSigned { get ;  }
        public string AccountLegalEntityName { get ;  }
        public AgreementType AgreementType { get ; }
    }
}