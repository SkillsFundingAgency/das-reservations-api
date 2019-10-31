﻿using System;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Nest;

namespace SFA.DAS.Reservations.Data.Repository
{
    public class ReservationIndexRepository : IReservationIndexRepository
    {
        public const string ReservationIndexLookupName = "reservations-index-registry";

        private readonly IElasticClient _client;

        public ReservationIndexRepository(IElasticClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ReservationIndex>> Find(long providerId, string term)
        {
            var searchIndexRegistryResponse = _client.Search<IndexRegistryEntry>(s => s
                .Index(ReservationIndexLookupName)
                .From(0)
                .Size(1)
                .Sort(x => x.Descending(a => a.DateCreated)));

            if (searchIndexRegistryResponse?.Documents == null || 
                !searchIndexRegistryResponse.Documents.Any())
            {
                return new ReservationIndex[0];
            }

            var reservationIndexName = searchIndexRegistryResponse.Documents.First().Name;

            if (string.IsNullOrEmpty(reservationIndexName))
            {
                return new ReservationIndex[0];
            }

            var searchResponse = await _client.SearchAsync<ReservationIndex>(s => s
                .Index(reservationIndexName)
                .From(0)
                .Size(10)
                .Query(q =>
                    q.Bool(b => b
                        .Must(x => x.Match(m => m.Field(f => f.IndexedProviderId).Query(providerId.ToString())))
                        .Filter(x => x.QueryString(descriptor => descriptor
                            .Fields(fields => fields.Field(f => f.CourseTitle)
                                                    .Field(f => f.AccountLegalEntityName))
                            .Query($"*{term}*")))
                    )));

            return searchResponse.Documents;
        }

        private class IndexRegistryEntry
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}
