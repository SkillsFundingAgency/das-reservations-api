using System;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationRequest
    {
        Guid Id { get; }
        long AccountId { get; }
        DateTime StartDate { get; }
        string CourseId { get;  }
        int? ProviderId { get; }
        long? AccountLegalEntityId { get; }
        string AccountLegalEntityName { get; }
        bool IsLevyAccount { get; }
    }
}
