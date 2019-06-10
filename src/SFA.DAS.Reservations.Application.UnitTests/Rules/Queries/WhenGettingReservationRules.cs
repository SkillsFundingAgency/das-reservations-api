using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Rules;
using GlobalRule = SFA.DAS.Reservations.Domain.Rules.GlobalRule;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    public class WhenGettingReservationRules
    {
        private GetRulesQueryHandler _handler;
        private CancellationToken _cancellationToken;
        private GetRulesQuery _query;
        private Mock<IRulesService> _service;
        private Mock<IGlobalRulesService> _globalRuleService;
        private List<ReservationRule> _rules;
        private List<GlobalRule> _globalRules;
        private const long ExpectedReservationRuleId = 553234;
        private const long ExpectedReservationGlobalRuleId = 4327;

        [SetUp]
        public void Arrange()
        {
            _query = new GetRulesQuery();
            _cancellationToken = new CancellationToken();
            _service = new Mock<IRulesService>();
            _globalRuleService = new Mock<IGlobalRulesService>();
            _globalRules = new List<GlobalRule>();
            _globalRuleService.Setup(x => x.GetAllRules())
                              .ReturnsAsync(_globalRules);

            _rules = new List<ReservationRule>
            {
                new ReservationRule(new Rule{Id = ExpectedReservationRuleId, Course = new Course()})
            };

            _handler = new GetRulesQueryHandler(_service.Object, _globalRuleService.Object);
        }
         
        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsAssignableFrom<GetRulesResult>(actual);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_With_The_Request_Details()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _service.Verify(x => x.GetRules());
        }

        [Test]
        public async Task Then_The_Values_Are_Returned_In_The_Response()
        {
            //Arrange
            _service.Setup(x => x.GetRules()).ReturnsAsync(_rules);

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsNotNull(actual.Rules);
            Assert.AreEqual(ExpectedReservationRuleId, actual.Rules[0].Id);
        }

        [Test]
        public async Task Then_If_There_Are_Global_Rules_Theses_Are_The_Only_Rules_Returned_In_The_Response()
        {
            //Arrange
            _globalRules = new List<GlobalRule>
            {
                new GlobalRule(new Domain.Entities.GlobalRule
                {
                    Id= ExpectedReservationGlobalRuleId,
                    RuleType = 0,
                    Restriction = 0,
                    ActiveFrom = DateTime.UtcNow
                })
            };
            _globalRuleService.Setup(x => x.GetAllRules()).ReturnsAsync(_globalRules);

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            _globalRuleService.Verify(x=>x.GetAllRules(), Times.Once);
            
            Assert.IsNotNull(actual.GlobalRules);
            _service.Verify(x=>x.GetRules(),Times.Never);
            Assert.IsNull(actual.Rules);
            Assert.AreEqual(ExpectedReservationGlobalRuleId, actual.GlobalRules[0].Id);
        }
    }
}
