using System;

namespace SFA.DAS.Reservations.NServiceBusListener
{
    public class EnvironmentVariables
    {
        public static string NServiceBusConnectionString = Environment.GetEnvironmentVariable("NServiceBusConnectionString");
        public static string NServiceBusLicense = Environment.GetEnvironmentVariable("NServiceBusLicense");
    }
}
