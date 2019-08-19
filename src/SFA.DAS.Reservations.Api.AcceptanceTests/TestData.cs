using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    public class TestData
    {
        public Dictionary<string, Course> Courses { get; set; }
        public bool IsLevyAccount { get; set; }

        public TestData()
        {
            Courses = new Dictionary<string, Course>();
        }
    }
}
