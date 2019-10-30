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
                .Size(10)
                .Query(q => 
                    q.Bool(b => b
                        .Must(x => x.Match(m => m.Field(f => f.IndexedProviderId).Query(providerId.ToString())))
                        .Filter(x => x.Wildcard(f => f.CourseTitle, $"*{term}*") ||
                                     x.Wildcard(f => f.AccountLegalEntityName, $"*{term}*"))
                        
                    )));

            return searchResponse.Documents;
        }
    }
}
