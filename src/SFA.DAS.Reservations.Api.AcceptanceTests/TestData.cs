using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    public class TestData
    {
        public Course Course { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public bool IsLevyAccount { get; set; }
    }
}
