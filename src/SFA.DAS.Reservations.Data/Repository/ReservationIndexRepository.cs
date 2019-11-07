using System;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nest;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public const string ReservationIndexLookupName = "-reservations-index-registry";

        private readonly IElasticClient _client;
        private readonly ReservationsApiEnvironment _environment;

        public ReservationIndexRepository(IElasticClient client, ReservationsApiEnvironment environment)
        {
            _client = client;
            _environment = environment;
        }

        public async Task<IndexedReservationSearchResult> Find(long providerId, string term, ushort pageNumber, ushort pageItemCount)
        {
            var searchIndexRegistryResponse = _client.Search<IndexRegistryEntry>(s => s
                .Index(_environment.EnvironmentName + ReservationIndexLookupName)
                .From(0)
                .Size(1)
                .Sort(x => x.Descending(a => a.DateCreated)));

            if (searchIndexRegistryResponse?.Documents == null || 
                !searchIndexRegistryResponse.Documents.Any())
            {
                return new IndexedReservationSearchResult();
            }

            var reservationIndexName = searchIndexRegistryResponse.Documents.First().Name;

            var formDocumentCount = pageNumber < 1 ? 0 : pageNumber * pageItemCount;

            if (string.IsNullOrEmpty(reservationIndexName))
            {
                return new IndexedReservationSearchResult();
            }

            ISearchResponse<ReservationIndex> searchResponse;

            if (string.IsNullOrEmpty(term))
            {
                searchResponse = await _client.SearchAsync<ReservationIndex>(s => s
                    .Index(reservationIndexName)
                    .From(formDocumentCount)
                    .Size(pageItemCount)
                    .Query(q =>
                        q.Bool(b => b
                            .Must(x => x.Match(m => m.Field(f => f.IndexedProviderId).Query(providerId.ToString())))
                        ))
                    .Sort(ss => ss.Ascending(f => f.AccountLegalEntityName.Suffix("keyword"))
                        .Ascending(index => index.CourseTitle.Suffix("keyword"))
                        .Descending(index => index.StartDate)));
            }
            else
            {
                var searchTermWords = term.Split(' ');

                var formattedSearchTerm = searchTermWords.Length > 1
                    ? searchTermWords.Aggregate((x, y) => $"*{x.Trim()}* AND *{y.Trim()}*")
                    : $"*{term}*";

                searchResponse = await _client.SearchAsync<ReservationIndex>(s => s
                    .Index(reservationIndexName)
                    .From(formDocumentCount)
                    .Size(pageItemCount)
                    .Query(q =>
                        q.Bool(b => b
                            .Must(x => x.Match(m => m.Field(f => f.IndexedProviderId).Query(providerId.ToString())))
                            .Filter(x => x.QueryString(descriptor => descriptor
                                .Fields(fields => fields.Field(f => f.CourseName)
                                    .Field(f => f.AccountLegalEntityName))
                                .Query(formattedSearchTerm)))
                        )).Sort(ss => ss.Ascending(f => f.AccountLegalEntityName.Suffix("keyword"))
                        .Ascending(index => index.CourseTitle.Suffix("keyword"))
                        .Descending(index => index.StartDate)));
            }

            return new IndexedReservationSearchResult(); //searchResponse.Documents;
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
