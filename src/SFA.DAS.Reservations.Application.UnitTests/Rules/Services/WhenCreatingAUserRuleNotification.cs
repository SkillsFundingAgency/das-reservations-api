using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Commands.CreateUserRuleAcknowledgement;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenCreatingAUserRuleNotification
    {
        private Mock<IUserRuleAcknowledgementRepository> _userRuleAcknowledgementRepository;
        private readonly Guid _expectedUserId = Guid.NewGuid();
        private UserRuleAcknowledgementService _userRuleAcknowledgementService;

        [SetUp]
        public void Arrange()
        {
           _userRuleAcknowledgementRepository = new Mock<IUserRuleAcknowledgementRepository>();

            _userRuleAcknowledgementRepository
                .Setup(x => x.Add(It.IsAny<Domain.Entities.UserRuleNotification>()))
                .ReturnsAsync(new Domain.Entities.UserRuleNotification { Id = 54353, UserId = _expectedUserId});

            _userRuleAcknowledgementService = new UserRuleAcknowledgementService(_userRuleAcknowledgementRepository.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_To_Create_A_UserRuleNotification_Mapping_To_The_Entity_From_The_Domain()
        {
            //Arrange
            var createUserRuleNotification = new CreateUserRuleAcknowledgementCommand
            {
                Id = _expectedUserId.ToString(),
                RuleId = 1,
                TypeOfRule = RuleType.GlobalRule
            };

            //Act
            await _userRuleAcknowledgementService.CreateUserRuleAcknowledgement(createUserRuleNotification);

            _userRuleAcknowledgementRepository.Verify(x => x.Add(It.Is<Domain.Entities.UserRuleNotification>(c =>
                                c.UserId.Value.Equals(_expectedUserId) &&
                                c.GlobalRuleId.Value.Equals(1))));
        }

        [Test]
        public async Task Then_The_UserRuleNotification_Is_Returned_Mapped_From_The_Entity()
        {
            //Arrange
            var createUserRuleNotification = new CreateUserRuleAcknowledgementCommand
            {
                Id = _expectedUserId.ToString(),
                RuleId = 1,
                TypeOfRule = RuleType.GlobalRule
            };

            //Act
            var actual = await _userRuleAcknowledgementService.CreateUserRuleAcknowledgement(createUserRuleNotification);

            //Assert
            actual.Should().BeAssignableTo<UserRuleAcknowledgement>();
            actual.UserId.Should().Be(_expectedUserId);
        }   
    }
}
