using System.IO;

namespace SFA.DAS.Reservations.Data.ElasticSearch
{
    public class ElasticSearchQueries : IElasticSearchQueries
    {
        public string LastIndexSearchQuery { get; }
        public string GetFilterValuesQuery { get; }
        public string FindReservationsQuery { get; }
        public string GetAllReservationsQuery { get; }

        public ElasticSearchQueries()
        {
            LastIndexSearchQuery = File.ReadAllText("ElasticSearch/LatestIndexSearchQuery.json");
            GetFilterValuesQuery = File.ReadAllText("ElasticSearch/GetFilterValuesQuery.json");
            FindReservationsQuery = File.ReadAllText("ElasticSearch/FindReservationsQuery.json");
            GetAllReservationsQuery = File.ReadAllText("ElasticSearch/GetAllReservationsQuery.json");
        }
    }
}
