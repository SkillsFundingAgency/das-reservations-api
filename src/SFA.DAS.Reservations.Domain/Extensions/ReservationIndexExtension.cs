using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Domain.Extensions
{
    public static class ReservationIndexExtension
    {
        public static Reservation ToReservation(this ReservationIndex index)
        {
            return new Reservation(
                null, 
                index.ReservationId,
                index.AccountId,
                index.IsLevyAccount,
                index.CreatedDate,
                index.StartDate,
                index.ExpiryDate,
                (ReservationStatus)index.Status,
                index.Course,
                index.ProviderId,
                index.AccountLegalEntityId,
                index.AccountLegalEntityName,
                index.TransferSenderAccountId,
                index.UserId);
        }
    }
}
