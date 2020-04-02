using System;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow.Assist;
using static System.String;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.ValueComparers
{
    public class CustomShortValueComparer : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is short;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            if (Enum.TryParse(expectedValue, true, out ReservationStatus status))
            {
                return (short)status == (short)actualValue;
            }

            if (short.TryParse(expectedValue, out var expected) == false)
                return false;
            return expected == (short) actualValue;
        }
    }
}