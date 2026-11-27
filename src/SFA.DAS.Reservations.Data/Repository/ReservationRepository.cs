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
    public class ReservationRepository(IReservationsDataContext reservationsDataContext) : IReservationRepository
    {
        public async Task<IList<Reservation>> GetAccountReservations(long accountId)
        {
            var result = await reservationsDataContext.Reservations
                .AsNoTracking()
                .Include(x => x.Course)
                .Where(c=>
                    c.AccountId.Equals(accountId) &&
                    !c.IsLevyAccount &&
                    c.Status != (int)ReservationStatus.Deleted &&
                    c.Status != (int)ReservationStatus.Change)
                .ToListAsync();
            
            return result;
        }

        public Task<int> CountPendingNonLevyReservations(long accountId)
        {
            var now = DateTime.UtcNow;

            return reservationsDataContext.Reservations.CountAsync(reservation =>
                reservation.AccountId == accountId
                && !reservation.IsLevyAccount
                && reservation.Status == (int)ReservationStatus.Pending
                && (reservation.ExpiryDate == null || reservation.ExpiryDate >= now));
        }

        public Task<int> CountReservationsTowardsLimit(long accountId, DateTime resetReservationDate)
        {
            var now = DateTime.UtcNow;

            return reservationsDataContext.Reservations.CountAsync(reservation =>
                reservation.AccountId == accountId
                && !reservation.IsLevyAccount
                && reservation.Status != (int)ReservationStatus.Deleted
                && reservation.Status != (int)ReservationStatus.Change
                && reservation.CreatedDate >= resetReservationDate
                && (reservation.Status != (int)ReservationStatus.Pending
                    || reservation.ExpiryDate == null
                    || reservation.ExpiryDate >= now));
        }

        public async Task<Reservation> CreateAccountReservation(Reservation reservation)
        {
            var reservationResult = await reservationsDataContext.Reservations.AddAsync(reservation);

            reservationsDataContext.SaveChanges();

            return reservationResult.Entity;
        }

        public async Task<Reservation> GetById(Guid id)
        {
            var reservationResult = await reservationsDataContext.Reservations.FindAsync(id);

            return reservationResult;
        }

        public async Task DeleteAccountReservation(Guid reservationId)
        {
            var reservationToDelete = await reservationsDataContext.Reservations.FindAsync(reservationId);

            if (reservationToDelete == null)
                throw new EntityNotFoundException<Reservation>();

            if (reservationToDelete.Status == (int) ReservationStatus.Confirmed ||
                reservationToDelete.Status == (int) ReservationStatus.Completed)
            {
                throw new InvalidOperationException("This reservation cannot be deleted");
            }
            
            reservationToDelete.Status = (int) ReservationStatus.Deleted;

            reservationsDataContext.SaveChanges();
        }

        public async Task CreateAccountReservations(List<Reservation> reservations)
        {
            await reservationsDataContext.Reservations.AddRangeAsync(reservations);

            reservationsDataContext.SaveChanges();
        }
    }
}
