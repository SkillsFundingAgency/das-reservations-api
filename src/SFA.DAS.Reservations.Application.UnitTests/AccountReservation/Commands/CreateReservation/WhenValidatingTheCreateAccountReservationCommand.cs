using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Courses;
using SFA.DAS.Reservations.Infrastructure.Configuration;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.CreateReservation
{
    public class WhenValidatingTheCreateAccountReservationCommand
    {
        private CreateAccountReservationValidator _validator;
        private Mock<ICourseService> _courseService;
        private CurrentDateTime _currentDateTime;

        [SetUp]
        public void Arrange()
        {
            _courseService = new Mock<ICourseService>();
            _currentDateTime = new CurrentDateTime(new DateTime(2019, 8, 8));

            _validator = new CreateAccountReservationValidator(_courseService.Object, _currentDateTime);

            _courseService.Setup(s => s.GetCourseById("1"))
                .ReturnsAsync(new Course(new Domain.Entities.Course()));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", 0, null,null, 0,"", false)]
        [TestCase("00000000-0000-0000-0000-000000000000", 1, "2019-08-08","TestName",0,"1", false)]
        [TestCase("1ef0c978-9800-4da7-ad90-1345c71f20aa", 1, null,"TestName", 0,"1" ,false)]
        [TestCase("0bc21b1f-41e0-4470-94ea-b6fda27c4300", 0, "2019-08-08","TestName", 0,"1", false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","", 0, "1", false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","TestName", 0, "1", false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","TestName", 1, "", false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","TestName", 1, "1", true)]
        public async Task Then_The_Command_Is_Validated_For_Each_Parameter(string id, long accountId, string date,string accountLegalEntityName,long accountLegalEntityId, string courseId, bool expected)
        {
            //Arrange
            var startDate = DateTime.TryParse(date, out var dateParsed);
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.Parse(id),
                AccountId = accountId,
                StartDate = startDate ? dateParsed : DateTime.MinValue,
                AccountLegalEntityName = accountLegalEntityName,
                AccountLegalEntityId = accountLegalEntityId,
                CourseId = courseId
            });

            //Assert
            Assert.AreEqual(expected, actual.IsValid());
        }

        [Test]
        public async Task Then_The_Commands_Required_Parameters_Are_Validated_And_Error_Messages_Returned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("Id has not been supplied"));
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("AccountId has not been supplied"));
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("StartDate has not been supplied"));
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("AccountLegalEntityName has not been supplied"));
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("AccountLegalEntityId has not been supplied"));
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("CourseId has not been supplied"));
        }

        [Test]
        public async Task Then_The_Command_Is_Valid_When_All_Required_Parameters_Are_Populated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 5432,
                StartDate = _currentDateTime.GetDate(),
                AccountLegalEntityName = "TestName",
                AccountLegalEntityId = 1,
                CourseId = "1"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.AreEqual(0, actual.ValidationDictionary.Count);
        }

        [Test]
        public async Task Then_The_Command_Is_Valid_If_Course_Exists()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                StartDate = _currentDateTime.GetDate(),
                CourseId = "1",
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "TestName"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task Then_If_The_Course_Has_Not_Been_Supplied_It_Is_Not_Checked_To_See_If_Exists()
        {
            //Arrange
            _courseService.Setup(s => s.GetCourseById(It.IsAny<string>()))
                .ReturnsAsync(new Course(new Domain.Entities.Course()));

            //Act
            await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                StartDate = DateTime.Now,
                CourseId = "",
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "TestName"
            });

            //Assert
            _courseService.Verify(x=>x.GetCourseById(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Then_The_Command_Is_InValid_If_Course_Does_Not_Exists()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id=Guid.NewGuid(),
                AccountId = 1,
                AccountLegalEntityId = 1,
                StartDate = _currentDateTime.GetDate(),
                CourseId = "2",
                AccountLegalEntityName = "TestName"
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("Course with CourseId cannot be found"));
        }

        [Test]
        public async Task Then_If_The_Command_Is_For_A_Levy_Reservation_The_Course_And_Start_Date_are_Not_Required()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 5432,
                AccountLegalEntityId = 1,
                IsLevyAccount = true
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.AreEqual(0, actual.ValidationDictionary.Count);
        }

        [TestCase(-2)]
        [TestCase(3)]
        public async Task Then_If_The_Command_Is_For_A_NonLevy_Reservation_The_Start_Date_Must_Not_Be_Outside_This_Range(int monthsToAdd)
        {
            var startDate = _currentDateTime.GetDate().AddMonths(monthsToAdd);
            var firstOfMonthStartDate = new DateTime(startDate.Year, startDate.Month, 1);

            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 5432,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Apple & co",
                IsLevyAccount = false,
                StartDate = firstOfMonthStartDate,
                CourseId = "1",
            });

            //Assert
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Count.Should().Be(1);
            actual.ValidationDictionary.First().Value.Should()
                .StartWith("Training start date must be between the funding reservation dates");
        }

        [TestCase(-1)]
        //[TestCase(2)]
        public async Task Then_If_The_Command_Is_For_A_NonLevy_Reservation_The_Start_Date_Must_Be_Within_This_Range(int monthsToAdd)
        {
            var startDate = _currentDateTime.GetDate().AddMonths(monthsToAdd);
            var firstOfMonthStartDate = new DateTime(startDate.Year, startDate.Month, 1);

            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 5432,
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "Apple & co",
                IsLevyAccount = false,
                StartDate = firstOfMonthStartDate,
                CourseId = "1",
            });

            //Assert
            actual.IsValid().Should().BeTrue();
        }
    }
}