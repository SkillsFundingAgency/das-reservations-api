using System;
using System.Collections.Generic;
using System.Text;
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

        [SetUp]
        public void Arrange()
        {
            var globalRules = new List<GlobalRule>
            {
                new GlobalRule
                {
                    ActiveFrom = new DateTime(2018 , 01 ,01),
                    Restriction = 1,
                    RuleType = 1
                }
            };
            _context = new Mock<IReservationsDataContext>();
            _context.Setup(x => x.GlobalRules).ReturnsDbSet(globalRules);
            _globalRulesRepository = new GlobalRuleRepository(_context.Object);
        }

        [Test]
        public async Task Then_Global_Rules_Are_Returned_By_Date()
        {
            //Arrange
            var dateFrom = new DateTime(2018, 01, 01);

            //Act
            var actual = await _globalRulesRepository.GetGlobalRules(dateFrom);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public async Task Then_If_There_Are_No_Active_Rules_Then_An_Empty_List_Is_Returned()
        {
            //Arrange
            var dateFrom = new DateTime(2017, 01, 01);

            //Act
            var actual = await _globalRulesRepository.GetGlobalRules(dateFrom);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsEmpty(actual);
        }
    }
}
