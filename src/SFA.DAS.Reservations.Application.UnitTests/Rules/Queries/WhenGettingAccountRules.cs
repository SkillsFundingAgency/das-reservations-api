using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    public class WhenGettingAccountRules
    {
        private GetAccountRulesQueryHandler _handler;
        private CancellationToken _cancellationToken;
        private GetAccountRulesQuery _query;
        private Mock<IGlobalRulesService> _globalRuleService;
        private List<GlobalRule> _accountGlobalRules;
        private List<GlobalRule> _globalRules;
        private Mock<IValidator<GetAccountRulesQuery>> _validator;
        private const long ExpectedAccountReservationGlobalRuleId = 0;
        private const long ExpectedAccountId = 123554;

        [SetUp]
        public void Arrange()
        {
            _query = new GetAccountRulesQuery
            {
                AccountId = ExpectedAccountId
            };
            _cancellationToken = new CancellationToken();
            _globalRuleService = new Mock<IGlobalRulesService>();
            _accountGlobalRules = new List<GlobalRule>{new GlobalRule(new Domain.Entities.GlobalRule
            {
                Id= ExpectedAccountReservationGlobalRuleId,
                RuleType = (byte)GlobalRuleType.ReservationLimit,
                Restriction = (byte)AccountRestriction.Account
            })};
            _globalRules = new List<GlobalRule>();
            _globalRuleService.Setup(x => x.GetAccountRules(ExpectedAccountId)).ReturnsAsync(_accountGlobalRules);
            _globalRuleService.Setup(x => x.GetActiveRules(It.IsAny<DateTime>())).ReturnsAsync(_globalRules);

            _validator = new Mock<IValidator<GetAccountRulesQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountRulesQuery>()))
                .ReturnsAsync(new ValidationResult());

            _handler = new GetAccountRulesQueryHandler(_globalRuleService.Object, _validator.Object);
        }

        [Test]
        public void Then_The_Request_Is_Validated_And_An_Argument_Exception_Is_Returned_If_Not_Valid()
        {
            //Arrange
            _validator
                .Setup(x => x.ValidateAsync(It.Is<GetAccountRulesQuery>(c => c.AccountId.Equals(ExpectedAccountId))))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(_query, _cancellationToken));
        }

        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            actual.Should().BeAssignableTo<GetAccountRulesResult>();
        }

        [Test]
        public async Task Then_If_There_Are_Global_Rules_Theses_Are_The_Only_Rules_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            _globalRuleService.Verify(x => x.GetAccountRules(ExpectedAccountId), Times.Once);
            actual.GlobalRules.Should().NotBeNull();
            actual.GlobalRules[0].Id.Should().Be(ExpectedAccountReservationGlobalRuleId);
        }

        [Test]
        public async Task Then_The_Active_Global_Rules_Are_Read_And_Added_To_The_Response()
        {
            //Arrange
            _globalRules = new List<GlobalRule>{new GlobalRule(new Domain.Entities.GlobalRule
            {
                Id = 10,
                RuleType = (byte)GlobalRuleType.FundingPaused,
                Restriction = (byte)AccountRestriction.All
            })};
            _globalRuleService.Setup(x => x.GetActiveRules(It.IsAny<DateTime>())).ReturnsAsync(_globalRules);

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            _globalRuleService.Verify(x => x.GetAccountRules(ExpectedAccountId), Times.Once);
            _globalRuleService.Verify(x => x.GetActiveRules(It.IsAny<DateTime>()), Times.Once);
            actual.GlobalRules.Should().NotBeNull();
            actual.GlobalRules.Should().HaveCount(2);
        }

        [Test]
        public async Task Then_Global_Rules_Where_AccountId_Is_Exempt_Are_Filtered_Out()
        {
            //Arrange
            _globalRules = new List<GlobalRule>
            {
                new GlobalRule(new Domain.Entities.GlobalRule
                {
                    Id = 10,
                    RuleType = (byte)GlobalRuleType.DynamicPause,
                    Restriction = (byte)AccountRestriction.NonLevy,
                    GlobalRuleAccountExemptions = new List<Domain.Entities.GlobalRuleAccountExemption>
                    {
                        new Domain.Entities.GlobalRuleAccountExemption
                        {
                            GlobalRuleId = 10,
                            AccountId = ExpectedAccountId
                        }
                    }
                })
            };

            _globalRuleService.Setup(x => x.GetActiveRules(It.IsAny<DateTime>())).ReturnsAsync(_globalRules);

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            _globalRuleService.Verify(x => x.GetAccountRules(ExpectedAccountId), Times.Once);
            _globalRuleService.Verify(x => x.GetActiveRules(It.IsAny<DateTime>()), Times.Once);
            actual.GlobalRules.Should().NotBeNull();
            actual.GlobalRules.Should().HaveCount(1);
        }
    }
}
