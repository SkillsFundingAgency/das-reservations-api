using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    [TestFixture]
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

        [Test, AutoData]
        public async Task Then_The_Available_Dates_Service_Is_Called_And_Dates_Returned(
            long accountLegalEntityId,
            List<AvailableDateStartWindow> availableDateStartWindows)
        {
            //Arrange
            _availableDatesService
                .Setup(x => x.GetAvailableDates(accountLegalEntityId))
                .Returns(availableDateStartWindows);

            //Act
            var actual = await _handler.Handle(new GetAvailableDatesQuery{AccountLegalEntityId = accountLegalEntityId}, _cancellationToken);

            //Assert
            Assert.IsAssignableFrom<GetAvailableDatesResult>(actual);
            Assert.AreSame(availableDateStartWindows, actual.AvailableDates);
        }
    }
}