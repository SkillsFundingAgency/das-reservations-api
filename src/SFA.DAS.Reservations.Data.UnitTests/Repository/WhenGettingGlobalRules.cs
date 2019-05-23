using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingGlobalRules
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
        public async Task Then_All_Global_Rules_Are_Returned()
        {
            //Act
            var actual = await _globalRulesRepository.GetAll();

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
            Assert.IsTrue(actual.Contains(_activeRule));
            Assert.IsTrue(actual.Contains(_futureRule));
        }

        [Test]
        public async Task Then_Active_Global_Rules_Are_Returned_By_Date()
        {
            //Act
            var actual = await _globalRulesRepository.FindActive(_fromDate);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual.Contains(_activeRule));
        }

        [Test]
        public async Task Then_If_There_Are_No_Active_Rules_Then_An_Empty_List_Is_Returned()
        {
            //Act
            var actual = await _globalRulesRepository.FindActive(_fromDate);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.Count);
            Assert.IsTrue(actual.Contains(_activeRule));
        }
    }
}
