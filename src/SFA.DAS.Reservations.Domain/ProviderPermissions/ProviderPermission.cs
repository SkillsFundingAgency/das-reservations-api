namespace SFA.DAS.Reservations.Domain.ProviderPermissions
{
    public class ProviderPermission
    {
        public long AccountId { get; set; }
        public uint UkPrn { get; set; }
        public long AccountLegalEntityId { get; set; }
        public bool AgreementSigned { get; set; }
        public string AccountLegalEntityName { get; set; }
        public string AccountName { get; set; }
    }
}