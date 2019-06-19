using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Domain.ApprenticeshipCourse;
using SFA.DAS.Reservations.Domain.Courses;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.CreateReservation
{
    public class WhenValidatingTheCreateAccountReservationCommand
    {
        private CreateAccountReservationValidator _validator;
        private Mock<ICourseService> _courseService;

        [SetUp]
        public void Arrange()
        {
            _courseService = new Mock<ICourseService>();

            _validator = new CreateAccountReservationValidator(_courseService.Object);

            _courseService.Setup(s => s.GetCourseById(It.IsAny<string>()))
                .ReturnsAsync(new Course(new Domain.Entities.Course()));
        }

        [TestCase("00000000-0000-0000-0000-000000000000", 0, null,null, 0, false)]
        [TestCase("00000000-0000-0000-0000-000000000000", 1, "2019-08-08","TestName",0, false)]
        [TestCase("1ef0c978-9800-4da7-ad90-1345c71f20aa", 1, null,"TestName", 0, false)]
        [TestCase("0bc21b1f-41e0-4470-94ea-b6fda27c4300", 0, "2019-08-08","TestName", 0, false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","", 0, false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","TestName", 0, false)]
        [TestCase("59294909-4549-4cb6-b849-25a32a18aa79", 1, "2019-08-08","TestName", 1, true)]
        public async Task Then_The_Command_Is_Validated_For_Each_Parameter(string id, long accountId, string date,string accountLegalEntityName,long accountLegalEntityId, bool expected)
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
                AccountLegalEntityId = accountLegalEntityId
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
        }

        [Test]
        public async Task Then_The_Command_Is_Valid_When_All_Required_Parameters_Are_Populated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                Id = Guid.NewGuid(),
                AccountId = 5432,
                StartDate = DateTime.UtcNow,
                AccountLegalEntityName = "TestName",
                AccountLegalEntityId = 1
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
                StartDate = DateTime.Now,
                CourseId = "1",
                AccountLegalEntityId = 1,
                AccountLegalEntityName = "TestName"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task Then_The_Command_Is_InValid_If_Course_Does_Not_Exists()
        {
            //Assign
            _courseService.Setup(s => s.GetCourseById(It.IsAny<string>()))
                .ReturnsAsync((Course) null);

            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                AccountId = 1,
                StartDate = DateTime.Now,
                CourseId = "1"
            });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("Course with CourseId cannot be found"));
        }
    }
}