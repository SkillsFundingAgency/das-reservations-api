using MediatR;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Reservation
{
    public class WhenBulkCreatingAccountReservations
    {
        private ReservationsController _reservationsController;
        private Mock<IMediator> _mediator;
  


        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            
            _reservationsController = new ReservationsController(Mock.Of<ILogger<ReservationsController>>(), _mediator.Object)
            {
                ControllerContext = {HttpContext = _httpContext.Object,
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        ControllerName = "reservations"
                    }}
            };
        }
    }
}
