using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Data;
using SFA.DAS.Reservations.Domain.Reservations;
using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class DeleteReservationSteps: StepsBase
    {
        public DeleteReservationSteps(TestData testData, TestServiceProvider serviceProvider) : base(testData, serviceProvider)
        {

        }
    
        [When(@"I delete the reservation")]
        public void WhenIDeleteTheReservation()
        {
            var controller = Services.GetService<ReservationsController>();

            controller.Delete(TestData.ReservationId).Wait(5000);
        }
        
        [Then(@"the reservation should be deleted")]
        public void ThenTheReservationShouldBeDeleted()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.SingleOrDefault(r => r.Id.Equals(TestData.ReservationId));

            Assert.IsNotNull(reservation?.Status);
            Assert.AreEqual(ReservationStatus.Deleted, (ReservationStatus) reservation.Status);
        }

        [Then(@"the reservation should not be deleted")]
        public void ThenTheReservationShouldNotBeDeleted()
        {
            var dbContext = Services.GetService<ReservationsDataContext>();

            var reservation = dbContext.Reservations.SingleOrDefault(r => r.Id.Equals(TestData.ReservationId));

            Assert.IsNotNull(reservation?.Status);
            Assert.AreNotEqual(ReservationStatus.Deleted, (ReservationStatus) reservation.Status);
        }
    }
}
