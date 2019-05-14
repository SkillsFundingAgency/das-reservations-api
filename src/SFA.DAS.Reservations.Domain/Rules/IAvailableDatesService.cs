using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IAvailableDatesService
    {
        IList<AvailableDateStartWindow> GetAvailableDates(long accountLegalEntity);
    }
}