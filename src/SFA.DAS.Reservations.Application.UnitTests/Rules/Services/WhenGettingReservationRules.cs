using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Services;
using SFA.DAS.Reservations.Domain.Entities;
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
            _ruleRepository.Setup(x => x.GetReservationRules(It.IsAny<DateTime>())).ReturnsAsync(new List<Rule>{_rule});
            
            _service = new RulesService(_ruleRepository.Object);
        }

        [Test]
        public async Task Then_The_Reservations_Are_Taken_From_The_Repository_For_That_Account()
        {
            //Act
            var actual = await _service.GetRules();

            //Assert
            _ruleRepository.Verify(x => x.GetReservationRules(It.Is<DateTime>(c=>c.ToShortDateString().Equals(DateTime.UtcNow.ToShortDateString()))));
            actual.Should().NotBeNull();
        }

        [Test]
        public async Task Then_The_Results_Are_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _service.GetRules();

            //Act
            actual.Should().NotBeNull();
            var actualRule = actual.FirstOrDefault();
            actualRule.Id.Should().Be(_rule.Id);
            actualRule.ActiveFrom.Should().Be(_rule.ActiveFrom);
            actualRule.ActiveTo.Should().Be(_rule.ActiveTo);
            actualRule.CourseId.Should().Be(_rule.CourseId);
            actualRule.CreatedDate.Should().Be(_rule.CreatedDate);
            actualRule.Restriction.Should().Be((AccountRestriction)_rule.Restriction);
            actualRule.Course.Should().NotBeNull();
            actualRule.Course.CourseId.Should().Be(_rule.Course.CourseId);
            actualRule.Course.Level.Should().Be(_rule.Course.Level.ToString());
            actualRule.Course.Title.Should().Be(_rule.Course.Title);
        }
    }
}
