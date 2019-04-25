using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenGettingAvailableDates
    {
        private Mock<IOptions<ReservationsConfiguration>> _options;

        [Test]
        public void Then_The_List_is_Built_From_The_Configuration_Values()
        {
            //Arrange
            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value.ExpiryPeriodInMonths).Returns(8);
            _options.Setup(x => x.Value.ExpiryPeriodMinDate).Returns(new DateTime(2018,10,01));
            _options.Setup(x => x.Value.ExpiryPeriodMaxDate).Returns(new DateTime(2019,02,01));
            var service = new AvailableDatesService(_options.Object);

            //Act
            var actual = service.GetAvailableDates();
            
            //Assert
            Assert.AreEqual(5, actual.Count);
        }
    }
}
