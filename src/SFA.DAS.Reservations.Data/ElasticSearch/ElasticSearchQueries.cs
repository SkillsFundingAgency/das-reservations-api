using System.IO;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public class ElasticSearchQueries : IElasticSearchQueries
    {
        public string ReservationIndexLookupName => "-reservations-index-registry";
        public string LastIndexSearchQuery { get; } = File.ReadAllText("ElasticSearch/LatestIndexSearchQuery.json");
        public string GetFilterValuesQuery { get; } = File.ReadAllText("ElasticSearch/GetFilterValuesQuery.json");
        public string FindReservationsQuery { get; } = File.ReadAllText("ElasticSearch/FindReservationsQuery.json");
        public string GetAllReservationsQuery { get; } = File.ReadAllText("ElasticSearch/GetAllReservationsQuery.json");
        public string GetReservationCountQuery { get; } = File.ReadAllText("ElasticSearch/GetReservationCountQuery.json");
    }
}
