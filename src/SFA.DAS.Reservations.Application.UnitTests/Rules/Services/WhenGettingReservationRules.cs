using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Services
{
    public class WhenGettingReservationRules
    {
        private RulesService _service;
        private Mock<IRuleRepository> _ruleRepository;
        private Rule _rule;


        [SetUp]
        public void Arrange()
        {
            _rule = new Rule
            {
                Id = 123,
                ActiveFrom = DateTime.UtcNow,
                ActiveTo = DateTime.UtcNow.AddMonths(6),
                CourseId = "3",
                CreatedDate = DateTime.UtcNow.AddMonths(-1),
                Restriction = 0,
                Course = new Course
                {
                    CourseId = "2-123-3",
                    Level = 1,
                    Title = "Test Course"
                }
            };
            _ruleRepository = new Mock<IRuleRepository>();
            _ruleRepository.Setup(x => x.GetReservationRules(It.IsAny<DateTime>(), It.IsAny<DateTime?>())).ReturnsAsync(new List<Rule>{_rule});
            
            _service = new RulesService(_ruleRepository.Object);
        }

        [Test]
        public async Task Then_The_Reservations_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _service.GetRules();

            //Assert
            _ruleRepository.Verify(x => x.GetReservationRules(It.Is<DateTime>(c=>c.ToShortDateString().Equals(DateTime.UtcNow.ToShortDateString())),null));
            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task Then_The_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _service.GetRules();

            //Act
            Assert.IsNotNull(actual);
            var actualRule = actual.FirstOrDefault();
            Assert.IsNotNull(actualRule);
            Assert.AreEqual(_rule.Id, actualRule.Id);
            Assert.AreEqual(_rule.ActiveFrom, actualRule.ActiveFrom);
            Assert.AreEqual(_rule.ActiveTo, actualRule.ActiveTo);
            Assert.AreEqual(_rule.CourseId, actualRule.CourseId);
            Assert.AreEqual(_rule.CreatedDate, actualRule.CreatedDate);
            Assert.AreEqual((AccountRestriction)_rule.Restriction, actualRule.Restriction);
            Assert.AreEqual(_rule.Course.CourseId, actualRule.Course.CourseId);
            Assert.AreEqual(_rule.Course.Level.ToString(), actualRule.Course.Level);
            Assert.AreEqual(_rule.Course.Title, actualRule.Course.Title);
            
        }
    }
}
