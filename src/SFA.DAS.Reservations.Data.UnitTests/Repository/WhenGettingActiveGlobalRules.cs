using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    class WhenGettingActiveGlobalRules
    {
         private Mock<IReservationsDataContext> _context;
        private GlobalRuleRepository _globalRulesRepository;
        private GlobalRule _activeRule;
        private GlobalRule _futureRule;
        private DateTime _fromDate;

        [SetUp]
        public void Arrange()
        {
            _fromDate = new DateTime(2019, 01, 01);

            _activeRule = new GlobalRule
            {
                ActiveFrom = _fromDate.AddDays(-2),
                ActiveTo = _fromDate.AddDays(10),
                Restriction = 1,
                RuleType = 1
            };

            _futureRule = new GlobalRule
            {
                ActiveFrom = _fromDate.AddDays(2),
                ActiveTo = _fromDate.AddDays(20),
                Restriction = 1,
                RuleType = 1
            };

            var globalRules = new List<GlobalRule>
            {
                _activeRule,
                _futureRule
            };

            _context = new Mock<IReservationsDataContext>();
            _context.Setup(x => x.GlobalRules).ReturnsDbSet(globalRules);
            _globalRulesRepository = new GlobalRuleRepository(_context.Object);
        }

        [Test]
        public async Task Then_Active_Global_Rules_Are_Returned_By_Date()
        {
            //Act
            var actual = await _globalRulesRepository.FindActive(_fromDate);

            //Assert
            actual.Should().NotBeNull();
            actual.Count.Should().Be(1);
            actual.Should().Contain(_activeRule);
        }

        [Test]
        public async Task Then_If_There_Are_No_Active_Rules_Then_An_Empty_List_Is_Returned()
        {
            //Act
            _fromDate = DateTime.Today;
            var actual = await _globalRulesRepository.FindActive(_fromDate);

            //Assert
            actual.Should().BeEmpty();
        }
    }
}
