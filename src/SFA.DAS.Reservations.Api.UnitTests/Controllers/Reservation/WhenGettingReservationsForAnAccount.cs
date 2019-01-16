using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenGettingReservationsForAnAccount
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private GetAccountReservationsResult _accountReservationsResult;
        private const long ExpectedAccountId = 123234;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _accountReservationsResult = new GetAccountReservationsResult{Reservations= new List<Domain.Reservations.Reservation>()};
            
            _mediator.Setup(x => x.Send(It.Is<GetAccountReservationsQuery>(c => c.AccountId.Equals(ExpectedAccountId)),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_accountReservationsResult);

            _reservationsController = new ReservationsController(_mediator.Object);
            
        }

        [Test]
        public async Task Then_The_Reservations_Returned()
        {
            //Act
            var actual = await _reservationsController.GetAll(ExpectedAccountId);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualReservations = result.Value as List<Domain.Reservations.Reservation>;
            Assert.AreEqual(_accountReservationsResult.Reservations,actualReservations);
        }

        [Test]
        public async Task Then_If_A_Validation_Error_Is_Returned_A_Bad_Request_Is_Returned()
        {

        }
    }
}
