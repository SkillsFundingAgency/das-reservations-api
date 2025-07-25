using Azure.Search.Documents;
using SFA.DAS.Reservations.Domain.Reservations;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Reservations.Data.AzureSearch;

public static class AzureSearchOptionExtensions
{

    public static SearchOptions BuildPaging(this SearchOptions searchOptions, ushort pageNumber, ushort pageItemCount)
    {
        searchOptions.Skip = (pageNumber - 1) * pageItemCount;
        searchOptions.Size = pageItemCount;
        return searchOptions;
    }

    public static SearchOptions BuildSort(this SearchOptions searchOptions)
    {
        searchOptions.OrderBy.Add("AccountLegalEntityName asc");
        searchOptions.OrderBy.Add("CourseTitle asc");
        searchOptions.OrderBy.Add("ReservationPeriod desc");
        return searchOptions;
    }

    public static SearchOptions BuildFindFilters(this SearchOptions searchOptions, long providerId, SelectedSearchFilters selectedFilters)
    {
        var filter = $"IndexedProviderId eq {providerId} and Status ne 3 and Status ne 4";

        var filterParts = new List<string>();

        if (!string.IsNullOrWhiteSpace(selectedFilters.CourseFilter))
            filterParts.Add($"search.ismatch('{selectedFilters.CourseFilter}', 'CourseDescription', 'simple', 'all')");

        if (!string.IsNullOrWhiteSpace(selectedFilters.EmployerNameFilter))
            filterParts.Add($"search.ismatch('{selectedFilters.EmployerNameFilter}', 'AccountLegalEntityName', 'simple', 'all')");

        if (!string.IsNullOrWhiteSpace(selectedFilters.StartDateFilter))
            filterParts.Add($"search.ismatch('{selectedFilters.StartDateFilter}', 'ReservationPeriod', 'simple', 'all')");

        filter += filterParts.Any() ? " and " + string.Join(" and ", filterParts) : "";

        searchOptions.Filter = filter;

        return searchOptions;
    }

    public static SearchOptions BuildCountFilter(this SearchOptions searchOptions, long providerId)
    {
        searchOptions.Filter = $"IndexedProviderId eq {providerId} and Status ne 3 and Status ne 4";
        searchOptions.IncludeTotalCount = true;
        return searchOptions;
    }

    public static SearchOptions BuildGetFiltersFilterWithFacets(this SearchOptions searchOptions, long providerId)
    {
        searchOptions.Filter = $"IndexedProviderId eq {providerId} and Status ne 3 and Status ne 4";
        searchOptions.Facets.Add("CourseDescription,count:1000");
        searchOptions.Facets.Add("AccountLegalEntityName,count:1000");
        searchOptions.Facets.Add("ReservationPeriod,count:1000");
        searchOptions.IncludeTotalCount = true;
        return searchOptions;
    }
}
