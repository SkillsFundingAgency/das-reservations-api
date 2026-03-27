using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Types;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Domain.Extensions
{
    public static class ReservationIndexExtension
    {
        public static Reservation ToReservation(this ReservationIndex index)
        {
            Course course = null;

            if (!string.IsNullOrEmpty(index.CourseId) && 
                !string.IsNullOrEmpty(index.CourseTitle) && 
                index.CourseLevel.HasValue)
            {
                course = new Course
                {
                    CourseId = index.CourseId,
                    Title = index.CourseTitle,
                    Level = index.CourseLevel.Value,
                    LearningType = index.CourseLearningType.HasValue ? (LearningType)index.CourseLearningType.Value : 0
                };
            }

            return new Reservation(
                null, 
                index.ReservationId,
                index.AccountId,
                index.IsLevyAccount,
                index.CreatedDate,
                index.StartDate,
                index.ExpiryDate,
                (ReservationStatus)index.Status,
                course,
                index.ProviderId,
                index.AccountLegalEntityId,
                index.AccountLegalEntityName,
                index.TransferSenderAccountId,
                index.UserId);
        }
    }
}
