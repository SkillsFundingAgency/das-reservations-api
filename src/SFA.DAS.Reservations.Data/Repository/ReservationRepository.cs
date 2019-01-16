using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly IReservationsDataContext _reservationsDataContext;

        public ReservationRepository(IReservationsDataContext reservationsDataContext)
        {
            _reservationsDataContext = reservationsDataContext;
        }
        public async Task<IList<Reservation>> GetAccountReservations(long accountId)
        {
            var result = await _reservationsDataContext.Reservations.Where(c=>c.AccountId.Equals(accountId)).ToListAsync();
            
            return result;
        }

        public async Task<long> CreateAccountReservation(Reservation reservation)
        {
            var reservationResult = await _reservationsDataContext.Reservations.AddAsync(reservation);
            _reservationsDataContext.SaveChanges();

            return reservationResult.Entity.Id;
        }
    }
}
