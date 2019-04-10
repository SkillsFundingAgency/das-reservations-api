using System;
using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationCommand : IRequest<CreateAccountReservationResult>, IReservationRequest
    {
        public Guid Id { get; set; }
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public string CourseId { get; set; }
        public int? ProviderId { get; set; }
        public long? AccountLegalEntityId { get; set; }
        public string AccountLegalEntityName { get; set; }
    }
}
