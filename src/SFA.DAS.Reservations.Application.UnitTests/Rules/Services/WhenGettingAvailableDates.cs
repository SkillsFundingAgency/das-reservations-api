using System;
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
        public void Then_Uses_AvailableDates(
            [Frozen]Mock<ICurrentDateTime> currentDateTime,
            Mock<IOptions<ReservationsConfiguration>> mockOptions)
        {
            currentDateTime.Setup(x => x.GetDate()).Returns(DateTime.UtcNow);
            var config = mockOptions.Object.Value;
            var expectedDates = new AvailableDates(
                    DateTime.UtcNow,
                    config.NumberOfAvailableDates, 
                    config.AvailableDatesMinDate, 
                    config.AvailableDatesMaxDate)
                .Dates;

            var availableDatesService = new AvailableDatesService(mockOptions.Object, currentDateTime.Object);
            
            //Act
            var actualDates = availableDatesService.GetAvailableDates();
            
            //Assert
            actualDates.Should().BeEquivalentTo(expectedDates);
        }
        
        [Test, MoqAutoData]
        public void Then_Uses_The_Date_From_The_Current_Date_Service(
            [Frozen]Mock<ICurrentDateTime> currentDateTime,
            Mock<IOptions<ReservationsConfiguration>> mockOptions)
        {
            var expectedDateTime = DateTime.UtcNow.AddMonths(3);

            currentDateTime.Setup(x => x.GetDate()).Returns(expectedDateTime);
            var config = mockOptions.Object.Value;
            var expectedDates = new AvailableDates(
                    expectedDateTime,
                    config.NumberOfAvailableDates,
                    config.AvailableDatesMinDate,
                    config.AvailableDatesMaxDate
                    )
                .Dates;

            var availableDatesService = new AvailableDatesService(mockOptions.Object, currentDateTime.Object);

            //Act
            var actualDates = availableDatesService.GetAvailableDates();

            //Assert
            actualDates.Should().BeEquivalentTo(expectedDates);
        }
    }
}
