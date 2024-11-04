﻿namespace SFA.DAS.Reservations.Data.UnitTests.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveLineEndingsAndWhiteSpace(this string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return value;
            }
            string lineSeparator = ((char) 0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value.Replace("\r\n", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(lineSeparator, string.Empty)
                .Replace(paragraphSeparator, string.Empty);
        }
    }
}
