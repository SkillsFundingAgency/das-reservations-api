using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingRulesFromTheRepository
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private Data.Repository.RuleRepository _ruleRepository;
        
        [SetUp]
        public void Arrange()
        {
            var rules = new List<Rule>
            {
                new Rule
                {
                    ActiveFrom = new DateTime(2018 , 01 ,01),
                    ActiveTo = DateTime.UtcNow.AddDays(1),
                    Restriction = 1,
                    CreatedDate = DateTime.UtcNow,
                    CourseId = "10"
                },
                new Rule
                {
                    ActiveFrom = DateTime.UtcNow.AddDays(1),
                    ActiveTo = DateTime.UtcNow.AddDays(10),
                    Restriction = 1,
                    CreatedDate = DateTime.UtcNow,
                    CourseId = "10"
                },
                new Rule
                {
                    ActiveFrom = DateTime.UtcNow.AddDays(3),
                    ActiveTo = DateTime.UtcNow.AddDays(10),
                    Restriction = 1,
                    CreatedDate = DateTime.UtcNow,
                    CourseId = "10"
                }
            };
            
            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Rules).ReturnsDbSet(rules);
            _ruleRepository = new Data.Repository.RuleRepository(_reservationsDataContext.Object);
        }
        

        [Test]
        public async Task Then_The_Results_Are_Filtered_By_The_Dates_Supplied_And_Current_Date_Used_For_End_Date_If_Not_Supplied()
        {
            //Arrange
            var dateFrom = DateTime.UtcNow;

            //Act
            var actual = await _ruleRepository.GetReservationRules(dateFrom);

            //Assert
            Assert.AreEqual(1, actual.Count);
        }
        
        [Test]
        public async Task Then_Rules_That_Are_Not_Active_Are_Excluded_From_The_Results_By_The_Dates_Supplied()
        {
            //Arrange
            var dateFrom = DateTime.UtcNow.AddDays(2);

            //Act
            var actual = await _ruleRepository.GetReservationRules(dateFrom);

            //Assert
            Assert.AreEqual(2, actual.Count);
        }


        [Ignore("Read date from injected value instead to test this")]
        public async Task Then_An_Empty_List_Is_Returned_When_There_Are_No_Records()
        {
            //Act
            var actual = await _ruleRepository.GetReservationRules(DateTime.UtcNow.AddYears(5));//, DateTime.UtcNow.AddYears(5).AddMonths(6)

            //Assert
            Assert.IsEmpty(actual);
        }
    }
}
