using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenGettingAvailableDates
    {
        [Test, MoqAutoData]
        public void And_Account_Not_Eoi_Then_Uses_AvailableDates(
            long accountId,
            Mock<IOptions<ReservationsConfiguration>> mockOptions)
        {
            var config = mockOptions.Object.Value;
            var expectedDates = new AvailableDates(
                    config.ExpiryPeriodInMonths, 
                    config.AvailableDatesMinDate, 
                    config.AvailableDatesMaxDate)
                .Dates;

            var availableDatesService = new AvailableDatesService(mockOptions.Object);
            
            //Act
            var actualDates = availableDatesService.GetAvailableDates(accountId);
            
            //Assert
            actualDates.Should().BeEquivalentTo(expectedDates);
        }

        [Test, MoqAutoData]
        public void And_Account_Is_Eoi_Then_Uses_EoiAvailableDates(
            long accountId,
            Mock<IOptions<ReservationsConfiguration>> mockOptions)
        {
            var config = mockOptions.Object.Value;
            var expectedDates = new EoiAvailableDates()
                .Dates;
            config.EoiAccountIds += $",{accountId}";

            var availableDatesService = new AvailableDatesService(mockOptions.Object);
            
            //Act
            var actualDates = availableDatesService.GetAvailableDates(accountId);
            
            //Assert
            actualDates.Should().BeEquivalentTo(expectedDates);
        }
    }
}
