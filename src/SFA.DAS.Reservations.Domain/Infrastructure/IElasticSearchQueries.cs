namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IElasticSearchQueries
    {
        string ReservationIndexLookupName { get; }

        string LastIndexSearchQuery { get; }
        string GetFilterValuesQuery { get; }
        string FindReservationsQuery { get; }
        string GetAllReservationsQuery { get; }
        string GetReservationCountQuery { get; }
    }
}