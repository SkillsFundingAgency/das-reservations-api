namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public interface IElasticSearchQueries
    {
        string LastIndexSearchQuery { get; }
        string GetFilterValuesQuery { get; }
        string FindReservationsQuery { get; }
        string GetAllReservationsQuery { get; }
        string GetReservationCountQuery { get; }
    }
}