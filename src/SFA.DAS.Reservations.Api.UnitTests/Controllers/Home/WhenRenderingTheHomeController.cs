using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Home
{
    public class WhenRenderingTheHomeController
    {
        private ReservationController _reservationController;

        [SetUp]
        public void Arrange()
        {
            _reservationController = new ReservationController();
        }

        [Test]
        public async Task Then_The_Correct_Response_Code_Is_Returned()
        {
            //Act
            var actual = await _reservationController.Index();

            //Assert
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
        }
    }
}
