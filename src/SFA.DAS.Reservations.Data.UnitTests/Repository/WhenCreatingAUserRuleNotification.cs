using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenCreatingAUserRuleNotification
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private UserRuleNotificationRepository _userRuleNotificationRepository;

        [SetUp]
        public void Arrange()
        {
            var userRuleNotification = new List<UserRuleNotification>
            {
                new UserRuleNotification()
            };
            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.UserRuleNotifications).ReturnsDbSet(userRuleNotification);
            _reservationsDataContext.Setup(x =>
                x.UserRuleNotifications.AddAsync(It.IsAny<UserRuleNotification>(), It.IsAny<CancellationToken>()));

            _userRuleNotificationRepository = new UserRuleNotificationRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_The_Entity_Is_Added_To_The_Repository()
        {
            //Act
            await _userRuleNotificationRepository.Add(new UserRuleNotification());

            //Assert
            _reservationsDataContext.Verify(x=>x.UserRuleNotifications.AddAsync(It.IsAny<UserRuleNotification>(),It.IsAny<CancellationToken>()),Times.Once);
        }
    }
}
