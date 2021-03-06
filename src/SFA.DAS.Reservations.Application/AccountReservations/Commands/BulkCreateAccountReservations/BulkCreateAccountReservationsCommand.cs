﻿using MediatR;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations
{
    public class BulkCreateAccountReservationsCommand : IRequest<BulkCreateAccountReservationsResult>
    {
        public uint ReservationCount { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long? TransferSenderAccountId { get; set; }
    }
}
