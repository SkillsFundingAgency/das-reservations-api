using AutoFixture;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenBulkCreatingAccountReservationsWihtNonLevy
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
        private Mock<HttpContext> _httpContext;
        private BulkCreateReservationsWithNonLevyResult _bulkCreateAccountReservationsResult;
        private BulkCreateReservationsRequest _bulkReservation;

        [SetUp]
        public void Arrange()
        {
            var fixture = new AutoFixture.Fixture();
            _bulkReservation = fixture.Create<BulkCreateReservationsRequest>();

            _bulkCreateAccountReservationsResult = fixture.Create<BulkCreateReservationsWithNonLevyResult>();
           
            
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.Is<BulkCreateReservationsCommand>(c =>
                       c.Reservations == _bulkReservation.Reservations),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(_bulkCreateAccountReservationsResult);
            _httpContext = new Mock<HttpContext>();
            _reservationsController = new ReservationsController(Mock.Of<ILogger<ReservationsController>>(), _mediator.Object)
            {
                ControllerContext = {HttpContext = _httpContext.Object,
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        ControllerName = "reservations"
                    }}
            };
        }

        [Test]
        public async Task Then_The_New_ReservationIds_Are_Returned_When_Created()
        {
            //Act
            var actual = await _reservationsController.BulkCreate(_bulkReservation);

            //Assert
            _mediator.Verify(m => m.Send(It.IsAny<BulkCreateReservationsCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
