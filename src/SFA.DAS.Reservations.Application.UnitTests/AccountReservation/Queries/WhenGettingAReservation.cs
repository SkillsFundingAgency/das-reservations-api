using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenGettingAReservation
    {
        private readonly Guid _expectedReservationId = Guid.NewGuid();
        private GetReservationQuery _query;
        private Mock<IValidator<GetReservationQuery>> _validator;
        private CancellationToken _cancellationToken;
        private Mock<IAccountReservationService> _service;
        private GetReservationQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _query = new GetReservationQuery {Id = _expectedReservationId};
            _validator = new Mock<IValidator<GetReservationQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetReservationQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _cancellationToken = new CancellationToken();
            _service = new Mock<IAccountReservationService>();
            var reservation = new Reservation(Guid.Empty, 12345, DateTime.UtcNow, 1);
            _service.Setup(x => x.GetReservation(_expectedReservationId)).ReturnsAsync(reservation);

            _handler = new GetReservationQueryHandler(_validator.Object, _service.Object);
        }
        [Test]
        public async Task Then_The_Query_Is_Validated()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _validator.Verify(x => x.ValidateAsync(_query), Times.Once);

        }

        [Test]
        public void Then_If_The_Query_Fails_Validation_Then_An_Argument_Exception_Is_Thrown()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetReservationQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(_query, _cancellationToken));
        }

        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsAssignableFrom<GetReservationResponse>(actual);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_With_The_Request_Details()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _service.Verify(x => x.GetReservation(_expectedReservationId));
        }

        [Test]
        public async Task Then_The_Values_Are_Returned_In_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsNotNull(actual.Reservation);
            Assert.AreEqual(12345, actual.Reservation.AccountId);
        }
    }
}
