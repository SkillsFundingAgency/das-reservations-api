using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenGettingReservationRulesForAnAccount
    {
        private GetAccountReservationsQueryHandler _handler;
        private Mock<IValidator<GetAccountReservationsQuery>> _validator;
        private CancellationToken _cancellationToken;
        private GetAccountReservationsQuery _query;
        private Mock<IAccountReservationService> _service;
        private Mock<IRuleRepository> _ruleRepository;
        private Reservation _reservation;
        private const long ExpectedAccountId = 553234;

        [SetUp]
        public void Arrange()
        {
            _query = new GetAccountReservationsQuery{AccountId = ExpectedAccountId};
            _validator = new Mock<IValidator<GetAccountReservationsQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountReservationsQuery>()))
                .ReturnsAsync(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});
            _cancellationToken = new CancellationToken();
            _service = new Mock<IAccountReservationService>();

            _ruleRepository = new Mock<IRuleRepository>();
            _reservation = new Reservation(_ruleRepository.Object){AccountId = ExpectedAccountId};

            _handler = new GetAccountReservationsQueryHandler(_validator.Object, _service.Object);
        }

        [Test]
        public async Task Then_The_Query_Is_Validated()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _validator.Verify(x=>x.ValidateAsync(_query), Times.Once);

        }

        [Test]
        public void Then_If_The_Query_Fails_Validation_Then_An_Argument_Exception_Is_Thrown()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountReservationsQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>{{"",""}} });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(_query, _cancellationToken));
        }

        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsAssignableFrom<GetAccountReservationsResult>(actual);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_If_Valid_With_The_Request_Details()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _service.Verify(x=>x.GetAccountReservations(ExpectedAccountId));
        }

        [Test]
        public async Task Then_The_Values_Are_Returned_In_The_Response()
        {
            //Arrange
            _service.Setup(x => x.GetAccountReservations(ExpectedAccountId)).ReturnsAsync(new List<Reservation>{_reservation});

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            Assert.IsNotNull(actual.Reservations);
            Assert.AreEqual(ExpectedAccountId, actual.Reservations[0].AccountId);
        }
    }
}
