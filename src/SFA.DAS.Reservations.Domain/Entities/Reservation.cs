﻿using System;

namespace SFA.DAS.Reservations.Domain.Entities
{
    public class Reservation
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public bool IsLevyAccount { get; set; }
        public long? ApprenticeId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public long? VacancyId { get; set; }
        public short Status { get; set; }
    }
}
