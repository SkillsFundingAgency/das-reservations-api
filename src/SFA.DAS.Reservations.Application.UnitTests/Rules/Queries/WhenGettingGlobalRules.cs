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
    public class WhenGettingGlobalRules
    {
        private GetGlobalRulesQueryHandler _handler;
        private CancellationToken _cancellationToken;
        private GetGlobalRulesQuery _query;
        private Mock<IGlobalRulesService> _service;
        private List<GlobalRule> _rules;
        private const long ExpectedReservationGlobalRuleId = 553234;

        [SetUp]
        public void Arrange()
        {
            _query = new GetGlobalRulesQuery();
            _cancellationToken = new CancellationToken();
            _service = new Mock<IGlobalRulesService>();

            _rules = new List<GlobalRule>
            {
                new GlobalRule(new Domain.Entities.GlobalRule
                {
                    Id= ExpectedReservationGlobalRuleId,
                    RuleType = 0,
                    Restriction = 0,
                    ActiveFrom = DateTime.UtcNow
                })
            };

            _handler = new GetGlobalRulesQueryHandler(_service.Object);
        }
        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsAssignableFrom<GetGlobalRulesResult>(actual);
        }

        [Test]
        public async Task Then_The_Service_Is_Called_With_The_Request_Details()
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
            Assert.IsNotNull(actual.GlobalRules);
            Assert.AreEqual(ExpectedReservationGlobalRuleId, actual.GlobalRules[0].Id);
        }
    }
}
