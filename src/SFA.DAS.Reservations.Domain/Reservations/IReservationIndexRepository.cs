using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public interface IReservationIndexRepository
    {
        Task<IEnumerable<ReservationIndex>> Find(long providerId, string term);
    }
}
