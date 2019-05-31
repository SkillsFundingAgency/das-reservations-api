using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Commands;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCreatingAUserRuleNotification
    {
        private Mock<IUserRuleNotificationRepository> _userRuleNotificationRepository;
        private readonly Guid _expectedUserId = Guid.NewGuid();
        private UserRuleNotificationService _userRuleNotificationService;

        [SetUp]
        public void Arrange()
        {
           _userRuleNotificationRepository = new Mock<IUserRuleNotificationRepository>();

            _userRuleNotificationRepository
                .Setup(x => x.Add(It.Is<Domain.Entities.UserRuleNotification>(c => c.UserId.Equals(_expectedUserId))))
                .ReturnsAsync(new Domain.Entities.UserRuleNotification { Id = 54353, UserId = _expectedUserId});

            _userRuleNotificationService = new UserRuleNotificationService(_userRuleNotificationRepository.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_UserRuleNotification_Mapping_To_The_Entity_From_The_Domain()
        {
            //Arrange
            var createUserRuleNotification = new CreateUserRuleNotification
            {
                Id = _expectedUserId.ToString(),
                RuleId = 1,
                RuleType = RuleType.GlobalRule
            };

            //Act
            await _userRuleNotificationService.CreateUserRuleNotification(createUserRuleNotification);

            //Assert
            _userRuleNotificationRepository.Verify(x => x.Add(It.Is<Domain.Entities.UserRuleNotification>(c =>
                                c.UserId.Equals(_expectedUserId) &&
                                c.GlobalRuleId.Equals(1)
                                )));
        }

        [Test]
        public async Task Then_The_UserRuleNotification_Is_Returned_Mapped_From_The_Entity()
        {
            //Arrange
            var createUserRuleNotification = new CreateUserRuleNotification
            {
                Id = _expectedUserId.ToString(),
                RuleId = 1,
                RuleType = RuleType.GlobalRule
            };

            //Act
            var actual = await _userRuleNotificationService.CreateUserRuleNotification(createUserRuleNotification);

            //Assert
            Assert.IsAssignableFrom<UserRuleNotification>(actual);
            Assert.AreEqual(_expectedUserId, actual.UserId);
        }   
    }
}
