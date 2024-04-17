using System;
using System.Linq;
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
        public void Then_Uses_PreviousMonth(
            [Frozen]Mock<ICurrentDateTime> currentDateTime,
            Mock<IOptions<ReservationsConfiguration>> mockOptions
            )
        {
            currentDateTime.Setup(x => x.GetDate()).Returns(DateTime.UtcNow);
            var config = mockOptions.Object.Value;
            
            var availableDatesService = new AvailableDatesService(mockOptions.Object, currentDateTime.Object);
          
            //Act
            var actualDate = availableDatesService.GetPreviousMonth();
            var previousMonth = DateTime.UtcNow.AddMonths(-1);
            var nextMonth = DateTime.UtcNow.AddMonths(1);

            //Assert
            actualDate.Should().NotBeNull();
            actualDate.StartDate.Day.Should().Be(1);
            actualDate.StartDate.Month.Should().Be(previousMonth.Month);
            actualDate.EndDate.Month.Should().Be(nextMonth.Month);
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
        
        [Test,MoqAutoData]
        public void Then_If_The_CurrentDate_Exceeds_The_Max_Date_It_IsNot_Added(
            [Frozen]Mock<ICurrentDateTime> currentDateTime,
            Mock<IOptions<ReservationsConfiguration>> mockOptions)
        {
            var expectedDateTime = DateTime.Today.AddDays(1);

            currentDateTime.Setup(x => x.GetDate()).Returns(expectedDateTime);
            var config = mockOptions.Object.Value;
            config.AvailableDatesMinDate = null;
            config.AvailableDatesMaxDate = DateTime.Today;
            var availableDatesService = new AvailableDatesService(mockOptions.Object, currentDateTime.Object);

            //Act
            var actualDates = availableDatesService.GetAvailableDates();

            //Assert
            actualDates.Count().Should().Be(0);
        }
    }
}
