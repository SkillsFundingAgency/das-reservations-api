using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    public class WhenGettingAvailableReservationDates
    {
        private GetAvailableDatesQueryHandler _handler;
        private Mock<IAvailableDatesService> _availableDatesService;
        private CancellationToken _cancellationToken;
        
        [SetUp]
        public void Arrange()
        {
            _cancellationToken = new CancellationToken();
            _availableDatesService = new Mock<IAvailableDatesService>();

            _handler = new GetAvailableDatesQueryHandler(_availableDatesService.Object);
        }

        [Test]
        public async Task Then_The_Available_Dates_Service_Is_Called_And_Dates_Returned()
        {
            //Arrange
            _availableDatesService.Setup(x => x.GetAvailableDates()).Returns(new List<AvailableDateStartWindow> {new AvailableDateStartWindow()});

            //Act
            var actual = await _handler.Handle(new GetAvailableDatesQuery(), _cancellationToken);

            //Assert
            Assert.IsAssignableFrom<GetAvailableDatesResult>(actual);
            Assert.IsNotEmpty(actual.AvailableDates);

        }
        
    }
}