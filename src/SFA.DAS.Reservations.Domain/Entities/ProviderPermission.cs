namespace SFA.DAS.Reservations.Domain.Entities
{
    public class ProviderPermission
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public virtual AccountLegalEntity AccountLegalEntity { get; set; }
        public long UkPrn { get; set; }
        public bool CanCreateCohort { get; set; }
    }
}