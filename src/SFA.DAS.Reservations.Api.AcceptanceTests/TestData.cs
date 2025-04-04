﻿using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    public class TestData
    {
        public Course Course { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public bool IsLevyAccount { get; set; }
        public Guid ReservationId { get; set; }
        public Guid UserId { get; set; }
        public ProviderPermission ProviderPermission { get ; set ; }
        public Account Account { get ; set ; }
        public IActionResult ActualResult { get; set; }
        public object Request { get; set; }
    }
}
