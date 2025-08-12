using Azure.Core.Serialization;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Constants;
using SFA.DAS.Reservations.Domain.Infrastructure;
using SFA.DAS.Reservations.Domain.Reservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.AzureSearch;

public class AzureSearchHelper : IAzureSearchHelper
{
    private readonly SearchClient _searchClient;
    private readonly SearchIndexClient _searchIndexerClient;
    private const int MaxRetries = 2;
    private readonly TimeSpan _networkTimeout = TimeSpan.FromSeconds(1);
    private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(100);
    private ILogger<AzureSearchHelper> _logger;

    public AzureSearchHelper(ReservationsConfiguration configuration, ILogger<AzureSearchHelper> logger)
    {
        _logger = logger;

        var clientOptions = new SearchClientOptions
        {
            Serializer = new JsonObjectSerializer(new JsonSerializerOptions())
        };

        _searchClient = new SearchClient(
            new Uri(configuration.AzureSearchBaseUrl),
            AzureSearchIndex.IndexName,
            new ChainedTokenCredential(
                new ManagedIdentityCredential(options: new TokenCredentialOptions
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                }),
                new AzureCliCredential(options: new AzureCliCredentialOptions
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                }),
                new VisualStudioCredential(options: new VisualStudioCredentialOptions
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                }),
                new VisualStudioCodeCredential(options: new VisualStudioCodeCredentialOptions()
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                })),
            clientOptions);

        _searchIndexerClient = new SearchIndexClient(new Uri(configuration.AzureSearchBaseUrl), new DefaultAzureCredential());
    }

    public async Task<string> GetIndexName(CancellationToken cancellationToken)
    {
        var result = await _searchIndexerClient.GetIndexAsync(AzureSearchIndex.IndexName, cancellationToken);
        return result.Value.Name;
    }

    public async Task<IndexedReservationSearchResult> Find(long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount, SelectedSearchFilters selectedFilters)
    {
        _logger.LogInformation("Starting reservation search");
        var searchOptions = new SearchOptions()
            .BuildSort()
            .BuildPaging(pageNumber, pageItemCount)
            .BuildFindFilters(providerId, selectedFilters);

        searchOptions.IncludeTotalCount = true;
        searchOptions.SearchMode = SearchMode.All;
        searchOptions.QueryType = SearchQueryType.Simple;
        searchOptions.SearchFields.Add("AccountLegalEntityName,CourseDescription");

        var azureSearchTerm = BuildSearchTerm(searchTerm);
        var searchResultsTask = _searchClient.SearchAsync<SearchDocument>($"{azureSearchTerm}", searchOptions);

        var totalCountSearchOptions = new SearchOptions().BuildCountFilter(providerId);
        var totalRecordCountTask = _searchClient.SearchAsync<SearchDocument>("*", totalCountSearchOptions);

        await Task.WhenAll(searchResultsTask, totalRecordCountTask);

        var result = searchResultsTask.Result.Value.GetResults().ToList().Select(r => JsonSerializer.Deserialize<ReservationIndex>(r.Document.ToString())).ToList();

        var filterValues = await GetFilterValues(providerId);

        var searchResult = new IndexedReservationSearchResult
        {
            Reservations = result,
            TotalReservations = (uint)searchResultsTask.Result.Value.TotalCount,
            TotalReservationsForProvider = (int)totalRecordCountTask.Result.Value.TotalCount,
            Filters = new SearchFilters
            {
                CourseFilters = filterValues.Courses,
                EmployerFilters = filterValues.AccountLegalEntityNames,
                StartDateFilters = filterValues.StartDates
            }
        };

        return searchResult;
    }

    private async Task<FilterValues> GetFilterValues(long providerId)
    {
        _logger.LogInformation("Retrieving filter values for provider {ProviderId}", providerId);

        var filterValues = new FilterValues();
        var searchOptions = new SearchOptions()
            .BuildGetFiltersFilterWithFacets(providerId);

        var response = await _searchClient.SearchAsync<SearchDocument>("*", searchOptions);
        if (response.Value.GetResults().Any())
        {
            var facets = response.Value.Facets;
            if (facets != null)
            {
                if (facets.TryGetValue("CourseDescription", out var courseFacets))
                {
                    filterValues.Courses = courseFacets
                        .Select(f => f.Value.ToString())
                        .ToList();
                }
                if (facets.TryGetValue("AccountLegalEntityName", out var accountLegalEntityNameFacets))
                {
                    filterValues.AccountLegalEntityNames = accountLegalEntityNameFacets
                        .Select(f => f.Value.ToString())
                        .ToList();
                }
                if (facets.TryGetValue("ReservationPeriod", out var reservationPeriodFacets))
                {
                    filterValues.StartDates = reservationPeriodFacets
                        .Select(f => f.Value.ToString())
                        .ToList();
                }
            }
        }

        return filterValues;
    }

    private string BuildSearchTerm(string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return "*";
        }
        if (searchTerm.Contains(' '))
        {
            var searchTermArray = searchTerm.Split(' ');
            var newSearch = new StringBuilder();
            foreach (var s in searchTermArray)
            {
                newSearch.Append('+');
                newSearch.Append(s);
                newSearch.Append('*');
            }
            return newSearch.ToString();
        }
        return $"{searchTerm}*";
    }

    private struct FilterValues
    {
        public ICollection<string> Courses { get; set; }
        public ICollection<string> AccountLegalEntityNames { get; set; }
        public ICollection<string> StartDates { get; set; }
    }

}
