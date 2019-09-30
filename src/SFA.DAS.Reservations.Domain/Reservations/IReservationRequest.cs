using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationRequest
    {
        Guid Id { get; }
        long AccountId { get; }
        DateTime? StartDate { get; }
        string CourseId { get;  }
        uint? ProviderId { get; }
        long AccountLegalEntityId { get; }
        string AccountLegalEntityName { get; }
        bool IsLevyAccount { get; }
        DateTime CreatedDate { get; }
        long? TransferSenderAccountId { get; }
    }
}
