using System;
using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty
{
    public class ChangeOfPartyCommand : IRequest<ChangeOfPartyResult>
    {
        public Guid ReservationId { get; set; }
        public long? AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
    }
}