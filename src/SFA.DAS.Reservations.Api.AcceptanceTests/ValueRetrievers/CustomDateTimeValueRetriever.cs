using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.ValueRetrievers
{
    public class CustomDateTimeValueRetriever : IValueRetriever
    {
        public virtual DateTime GetValue(string value)
        {
            if (value == "today")
                return DateTime.Today;
            if (value == "utcnow")
                return DateTime.UtcNow;

            var returnValue = DateTime.MinValue;
            DateTime.TryParse(value, out returnValue);
            return returnValue;
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(DateTime);
        }
    }
}