using System;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenValidatingValidateReservation
    {
        private ValidateReservationValidator _validator;
        private ValidateReservationQuery _query;

        [SetUp]
        public void Arrange()
        {
            _validator = new ValidateReservationValidator();
            
            _query = new ValidateReservationQuery
            {
                ReservationId = Guid.NewGuid(),
                CourseCode = "1", 
                StartDate = DateTime.Now
            };
        }

        [Test]
        public async Task ThenWillPassValidationIfReservationIdIsValid()
        {
            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid());
            Assert.IsEmpty(result.ValidationDictionary);
        }

        [Test]
        public async Task ThenWillThrowErrorIfReservationIdIsInvalid()
        {
            //Arrange
            _query.ReservationId = Guid.Empty;

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(_query.ReservationId)));
        }

        [Test]
        public async Task ThenWillThrowErrorIfCourseIdIsInvalid()
        {
            //Arrange
            _query.CourseCode = "";

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(_query.CourseCode)));
        }

        [Test]
        public async Task ThenWillThrowErrorIfTrainingStartDateIsInvalid()
        {
            //Arrange
            _query.StartDate = default(DateTime);

            //Act
            var result = await _validator.ValidateAsync(_query);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(_query.StartDate)));
        }
    }
}
