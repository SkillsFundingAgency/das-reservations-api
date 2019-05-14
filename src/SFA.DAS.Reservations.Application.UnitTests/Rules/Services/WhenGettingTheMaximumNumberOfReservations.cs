using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenGettingTheMaximumNumberOfReservations
    {
        private GlobalRulesService _globalRulesService;
        private Mock<IOptions<ReservationsConfiguration>> _options;

        [Test]
        public void Then_It_Is_Returned_From_The_Configured_Value()
        {
            //Arrange
            const int maxNumberOfReservations = 90;
            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(maxNumberOfReservations);
            _globalRulesService = new GlobalRulesService(Mock.Of<IGlobalRuleRepository>(), _options.Object,
                Mock.Of<IReservationRepository>());

            //Act
            var actual = _globalRulesService.GetReservationLimit();

            //Assert
            Assert.AreEqual(maxNumberOfReservations, actual);
        }
    }
}