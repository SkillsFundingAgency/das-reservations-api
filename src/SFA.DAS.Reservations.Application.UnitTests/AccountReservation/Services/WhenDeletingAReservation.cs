using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    [TestFixture]
    public class WhenDeletingAReservation
    {
        [Test, MoqAutoData]
        public async Task Then_Calls_Repository_Delete(
            Guid reservationId,
            [Frozen] Mock<IReservationRepository> mockRepo,
            AccountReservationService service)
        {
            service.DeleteReservation(reservationId);

            mockRepo.Verify(repository => repository.DeleteAccountReservation(reservationId), 
                Times.Once);
        }
    }
}