using System;
using System.Collections.Generic;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.ValueRetrievers
{
    public class CustomShortValueRetriever : IValueRetriever
    {
        public virtual short GetValue(string value)
        {
            if (Enum.TryParse(value, true, out ReservationStatus status))
            {
                return (short)status;
            }

            short.TryParse(value, out var returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(short);
        }
    }
}