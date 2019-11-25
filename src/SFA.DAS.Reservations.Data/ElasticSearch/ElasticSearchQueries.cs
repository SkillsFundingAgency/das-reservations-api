using System.IO;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public class ElasticSearchQueries : IElasticSearchQueries
    {
        public string ReservationIndexLookupName => "-reservations-index-registry";
        public string LastIndexSearchQuery { get; }
        public string GetFilterValuesQuery { get; }
        public string FindReservationsQuery { get; }
        public string GetAllReservationsQuery { get; }
        public string GetReservationCountQuery { get; }

        public ElasticSearchQueries()
        {
            LastIndexSearchQuery = File.ReadAllText("ElasticSearch/LatestIndexSearchQuery.json");
            GetFilterValuesQuery = File.ReadAllText("ElasticSearch/GetFilterValuesQuery.json");
            FindReservationsQuery = File.ReadAllText("ElasticSearch/FindReservationsQuery.json");
            GetAllReservationsQuery = File.ReadAllText("ElasticSearch/GetAllReservationsQuery.json");
            GetReservationCountQuery = File.ReadAllText("ElasticSearch/GetReservationCountQuery.json");
        }
    }
}
