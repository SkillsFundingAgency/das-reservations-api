using System;
using System.Collections.Generic;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.ValueRetrievers
{
    public class CustomNullableDateTimeValueRetriever : IValueRetriever
    {
        private readonly Func<string, DateTime> dateTimeValueRetriever = v => new CustomDateTimeValueRetriever().GetValue(v);

        public CustomNullableDateTimeValueRetriever()
        {
            
        }

        public CustomNullableDateTimeValueRetriever(Func<string, DateTime> dateTimeValueRetriever = null)
        {
            if (dateTimeValueRetriever != null)
                this.dateTimeValueRetriever = dateTimeValueRetriever;
        }

        public DateTime? GetValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return dateTimeValueRetriever(value);
        }

        public object Retrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return GetValue(keyValuePair.Value);
        }

        public bool CanRetrieve(KeyValuePair<string, string> keyValuePair, Type targetType, Type propertyType)
        {
            return propertyType == typeof(DateTime?);
        }
    }
}