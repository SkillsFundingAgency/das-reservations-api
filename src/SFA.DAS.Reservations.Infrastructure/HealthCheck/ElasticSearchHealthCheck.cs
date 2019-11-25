using System;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.HealthCheck
{
    public class ElasticSearchHealthCheck : IHealthCheck
    {
        private readonly IElasticLowLevelClient _client;
        private readonly ReservationsApiEnvironment _environment;
        private readonly IElasticSearchQueries _elasticQueries;

        public ElasticSearchHealthCheck(IElasticLowLevelClient client, 
            ReservationsApiEnvironment environment, 
            IElasticSearchQueries elasticQueries)
        {
            _client = client;
            _environment = environment;
            _elasticQueries = elasticQueries;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            //_client.PingAsync<>()

            //var data = PostData.String(_elasticQueries.LastIndexSearchQuery);

            //_logger.LogDebug("Getting latest reservation index name");

            //var response = await _client.SearchAsync<StringResponse>(
            //    _environment.EnvironmentName + ReservationIndexLookupName, data);
            throw new NotImplementedException();
        }
    }
}
