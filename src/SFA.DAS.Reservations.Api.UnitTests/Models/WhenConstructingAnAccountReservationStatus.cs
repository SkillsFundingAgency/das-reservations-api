using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;

namespace SFA.DAS.Reservations.Api.UnitTests.Models
{
    [TestFixture]
    public class WhenConstructingAnAccountReservationStatus
    {
        [Test, AutoData]
        public void Then_Sets_CanAutoCreateReservations(
            GetAccountReservationStatusResponse response)
        {
            var model = new AccountReservationStatus(response);

            model.CanAutoCreateReservations.Should().Be(response.CanAutoCreateReservations);
        }
    }
}