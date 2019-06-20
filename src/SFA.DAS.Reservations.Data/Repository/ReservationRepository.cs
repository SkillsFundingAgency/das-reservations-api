using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Reservations.Domain.Exceptions;
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
            var result = await _reservationsDataContext.Reservations
                .Where(c=>c.AccountId.Equals(accountId) &&
                          c.Status != (int)ReservationStatus.Deleted)
                .ToListAsync();
            
            return result;
        }

        public async Task<Reservation> CreateAccountReservation(Reservation reservation)
        {
            var reservationResult = await _reservationsDataContext.Reservations.AddAsync(reservation);

            _reservationsDataContext.SaveChanges();

            return reservationResult.Entity;
        }

        public async Task<Reservation> GetById(Guid id)
        {
            var reservationResult = await _reservationsDataContext.Reservations.FindAsync(id);

            return reservationResult;
        }

        public async Task DeleteAccountReservation(Guid reservationId)
        {
            var reservationToDelete = await _reservationsDataContext.Reservations.FindAsync(reservationId);

            if (reservationToDelete == null)
                throw new EntityNotFoundException<Reservation>();

            if (reservationToDelete.Status == (int) ReservationStatus.Confirmed ||
                reservationToDelete.Status == (int) ReservationStatus.Completed)
            {
                throw new InvalidOperationException("This reservation cannot be deleted");
            }
            
            reservationToDelete.Status = (int) ReservationStatus.Deleted;

            _reservationsDataContext.SaveChanges();
        }

        public Task CreateAccountReservations(List<Reservation> reservations)
        {
            throw new NotImplementedException();
        }
    }
}
