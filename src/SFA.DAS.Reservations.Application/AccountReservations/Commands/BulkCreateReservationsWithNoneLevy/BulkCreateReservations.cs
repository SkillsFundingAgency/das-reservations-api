﻿using System;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNoneLevy
{
    public class BulkCreateReservations
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
        public string ULN { get; set; }
    }
}
