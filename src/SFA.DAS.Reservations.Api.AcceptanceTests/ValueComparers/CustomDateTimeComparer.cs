﻿using System;
using SFA.DAS.Reservations.Api.AcceptanceTests.ValueRetrievers;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.ValueComparers
{
    public class CustomDateTimeComparer(CustomDateTimeValueRetriever dateTimeValueRetriever) : IValueComparer
    {
        public bool CanCompare(object actualValue)
        {
            return actualValue is DateTime;
        }

        public bool Compare(string expectedValue, object actualValue)
        {
            if (DateTime.TryParse(expectedValue, out var expected) == false)
            {
                expected = dateTimeValueRetriever.GetValue(expectedValue);
                if (expected == DateTime.MinValue)
                {
                    return false;
                }
            }
            
            return expected.Date == ((DateTime) actualValue).Date;
        }
    }
}