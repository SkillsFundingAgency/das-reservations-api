using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.UnitTests.Rules
{
    [TestFixture]
    public class WhenGettingEoiAvailableDates
    {
        [Test]
        public void Then_Number_Defaults_To_6()
        {
            var expectedMaxDate = new DateTime(2019, 12, 1);

            var actualDates = new EoiAvailableDates(DateTime.UtcNow, null).Dates;

            actualDates.OrderBy(window => window.StartDate).Last().StartDate
                .Should().Be(expectedMaxDate);
        }

        [Test]
        public void Then_Min_Date_Defaults_To_Aug_2019()
        {
            var expectedMinDate = new DateTime(2019, 8, 1);

            var actualDates = new EoiAvailableDates(DateTime.UtcNow).Dates;

            actualDates.OrderBy(window => window.StartDate).First().StartDate
                .Should().Be(expectedMinDate);
        }

        [Test]
        public void Then_Max_Date_Defaults_To_Dec_2019()
        {
            var expectedMaxDate = new DateTime(2019, 12, 1);

            var actualDates = new EoiAvailableDates(DateTime.UtcNow).Dates;

            actualDates.OrderBy(window => window.StartDate).Last().StartDate
                .Should().Be(expectedMaxDate);
        }
    }
}