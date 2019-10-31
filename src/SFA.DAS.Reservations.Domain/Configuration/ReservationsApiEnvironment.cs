using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public class ReservationsApiEnvironment
    {
        public virtual string EnvironmentName { get; }

        public ReservationsApiEnvironment(string environmentName)
        {
            EnvironmentName = environmentName.ToLower();
        }
    }
}
