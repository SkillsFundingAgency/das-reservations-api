using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    public class WhenGettingReservationRules
    {
        private GetRulesQueryHandler _handler;
        private CancellationToken _cancellationToken;
        private GetRulesQuery _query;
        private Mock<IRulesService> _service;
        private List<ReservationRule> _rules;
        private const long ExpectedReservationRuleId = 553234;

        [SetUp]
        public void Arrange()
        {
            _query = new GetRulesQuery();
            _cancellationToken = new CancellationToken();
            _service = new Mock<IRulesService>();

            _rules = new List<ReservationRule>
            {
                new ReservationRule(new Rule{Id = ExpectedReservationRuleId, Course = new Course()})
            };

            _handler = new GetRulesQueryHandler(_service.Object);
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
        public async Task Then_The_Repository_Is_Called_If_Valid_With_The_Request_Details()
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
    }
}
