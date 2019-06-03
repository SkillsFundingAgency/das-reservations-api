using System;

namespace SFA.DAS.Reservations.Domain.Configuration
{
    public interface ICurrentDateTime
    {
        DateTime GetDate();
    }
}
