using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands
{
    public class WhenValidatingTheCreateAccountReservationCommand
    {
        private CreateAccountReservationValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new CreateAccountReservationValidator();
        }

        [TestCase(0, null, false)]
        [TestCase(1, null, false)]
        [TestCase(0, "2019-08-08", false)]
        [TestCase(1, "2019-08-08", true)]
        public async Task Then_The_Command_Is_Validated_For_Each_Parameter(long accountId, string date, bool expected)
        {
            //Arrange
            var startDate = DateTime.TryParse(date, out var dateParsed);
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                AccountId = accountId,
                StartDate = startDate ? dateParsed : DateTime.MinValue
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
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("AccountId has not been supplied"));
            Assert.IsTrue(actual.ValidationDictionary.ContainsValue("StartDate has not been supplied"));
        }

        [Test]
        public async Task Then_The_Command_Is_Valid_When_All_Required_Parameters_Are_Populated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new CreateAccountReservationCommand
            {
                AccountId = 5432,
                StartDate = DateTime.UtcNow
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
            Assert.AreEqual(0, actual.ValidationDictionary.Count);
        }
    }
}