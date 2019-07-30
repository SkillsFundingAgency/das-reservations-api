using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IAvailableDatesService
    {
        IEnumerable<AvailableDateStartWindow> GetAvailableDates(bool isEoi);
    }
}