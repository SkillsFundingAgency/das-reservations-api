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
        private ReservationController _reservationController;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _reservationController = new ReservationController(_mediator.Object);
        }

        [Test]
        public async Task Then_The_Correct_Response_Code_Is_Returned()
        {
            //Act
            var actual = await _reservationController.GetAll(54321);

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
        }

        [Test]
        public async Task Then_The_Request_Is_Made_Passing_The_AccountId()
        {
            //Arrange
            var expectedAccountId = 54321;

            //Act
            await _reservationController.GetAll(expectedAccountId);

            //Assert
            _mediator.Verify(x=>x.Send(It.Is<GetAccountReservationsQuery>(c=>c.AccountId.Equals(expectedAccountId)),It.IsAny<CancellationToken>()),Times.Once);
        }

        [Test]
        public async Task Then_The_Results_Are_Returned_In_The_Response()
        {

        }

        [Test]
        public async Task Then_A_Not_Found_Response_Code_Is_Returned()
        {

        }

        [Test]
        public async Task Then_If_A_Validation_Error_Is_Returned_A_Bad_Request_Is_Returned()
        {

        }
    }
}
