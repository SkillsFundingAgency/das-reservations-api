using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCreatingAUserRuleNotification
    {
        private Mock<IUserRuleAcknowledgementRepository> _userRuleNotificationRepository;
        private readonly Guid _expectedUserId = Guid.NewGuid();
        private UserRuleAcknowledgementService _userRuleAcknowledgementService;

        [SetUp]
        public void Arrange()
        {
           _userRuleNotificationRepository = new Mock<IUserRuleAcknowledgementRepository>();

            _userRuleNotificationRepository
                .Setup(x => x.Add(It.Is<Domain.Entities.UserRuleNotification>(c => c.UserId.Equals(_expectedUserId))))
                .ReturnsAsync(new Domain.Entities.UserRuleNotification { Id = 54353, UserId = _expectedUserId});

            _userRuleAcknowledgementService = new UserRuleAcknowledgementService(_userRuleNotificationRepository.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_UserRuleNotification_Mapping_To_The_Entity_From_The_Domain()
        {
            //Arrange
            var createUserRuleNotification = new CreateUserRuleAcknowledgementCommand
            {
                Id = _expectedUserId.ToString(),
                RuleId = 1,
                RuleType = RuleType.GlobalRule
            };

            //Act
            await _userRuleAcknowledgementService.CreateUserRuleAcknowledgement(createUserRuleNotification);

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
            var createUserRuleNotification = new CreateUserRuleAcknowledgementCommand
            {
                Id = _expectedUserId.ToString(),
                RuleId = 1,
                RuleType = RuleType.GlobalRule
            };

            //Act
            var actual = await _userRuleAcknowledgementService.CreateUserRuleAcknowledgement(createUserRuleNotification);

            //Assert
            Assert.IsAssignableFrom<UserRuleAcknowledgement>(actual);
            Assert.AreEqual(_expectedUserId, actual.UserId);
        }   
    }
}
