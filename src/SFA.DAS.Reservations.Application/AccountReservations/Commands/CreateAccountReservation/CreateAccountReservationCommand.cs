using System;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation
{
    public class CreateAccountReservationCommand : IRequest<CreateAccountReservationResult>, IReservationRequest
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public DateTime? StartDate { get; set; }
        public string CourseId { get; set; }
        public uint? ProviderId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public bool IsLevyAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid? UserId { get; set; }
    }
}
