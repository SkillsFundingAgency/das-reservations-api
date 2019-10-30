using SFA.DAS.Reservations.Domain.Reservations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        private readonly IElasticClient _client;

        public ReservationIndexRepository(IElasticClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ReservationIndex>> Find(long providerId, string term)
        {
            var searchResponse = await _client.SearchAsync<ReservationIndex>(s => s
                .From(0)
                .Size(2000)
                .Query(q => q.MatchAll()));

            return searchResponse.Documents;
        }
    }
}
