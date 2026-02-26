using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.Rules.Services;

public class AvailableDatesService(IOptions<ReservationsConfiguration> options, ICurrentDateTime currentDateTime)
    : IAvailableDatesService
{
    private readonly ReservationsConfiguration _configuration = options.Value;

    public IEnumerable<AvailableDateStartWindow> GetAvailableDates(bool includePreviousMonth = true)
    {
        var now = currentDateTime.GetDate();

        return new AvailableDates(
                now,
                _configuration.NumberOfAvailableDates,
                _configuration.AvailableDatesMinDate,
                _configuration.AvailableDatesMaxDate,
                includePreviousMonth)
            .Dates;
    }
}