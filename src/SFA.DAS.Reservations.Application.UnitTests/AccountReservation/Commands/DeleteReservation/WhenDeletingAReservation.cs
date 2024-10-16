using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.DeleteReservation;
using SFA.DAS.Reservations.Application.UnitTests.Customisations;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Messages;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.UnitOfWork.Context;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.DeleteReservation
{
    [TestFixture]
    public class WhenDeletingAReservation
    {
        [Test, MoqAutoData]
        public void And_Command_Fails_Validation_Then_Throws_ValidationException(
            DeleteReservationCommand command,
            string propertyName,
            [Frozen] ValidationResult validationResult,
            DeleteReservationCommandHandler handler)
        {
            validationResult.AddError(propertyName);

            var act = async () => await handler.Handle(command, CancellationToken.None);

            act.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"The following parameters have failed validation*{propertyName}*");
        }

        [Test, MoqAutoData]
        public async Task Then_Calls_Service_To_Delete_Reservation(
            DeleteReservationCommand command,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountReservationService> mockService,
            DeleteReservationCommandHandler handler)
        {
            validationResult.ValidationDictionary.Clear();

            await handler.Handle(command, CancellationToken.None);

            mockService
                .Verify(service => service.DeleteReservation(command.ReservationId),
                Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_An_Event_Is_Fired(
            DeleteReservationCommand command,
            [ReservationWithCourse] Reservation reservationToDelete,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountReservationService> mockService,
            [Frozen] Mock<IUnitOfWorkContext> mockContext,
            DeleteReservationCommandHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetReservation(command.ReservationId))
                .ReturnsAsync(reservationToDelete);

            await handler.Handle(command, CancellationToken.None);

            mockContext.Verify(x => x.AddEvent(It.Is<ReservationDeletedEvent>(e => 
                e.Id.Equals(command.ReservationId)
                && e.AccountId.Equals(reservationToDelete.AccountId)
                && e.AccountLegalEntityId.Equals(reservationToDelete.AccountLegalEntityId)
                && e.AccountLegalEntityName.Equals(reservationToDelete.AccountLegalEntityName)
                && e.ProviderId.Equals(reservationToDelete.ProviderId)
                && e.CourseId.Equals(reservationToDelete.Course.CourseId)
                && e.CourseName.Equals(reservationToDelete.Course.Title)
                && e.CourseLevel.Equals(reservationToDelete.Course.Level)
                && e.StartDate.Equals(reservationToDelete.StartDate)
                && e.EndDate.Equals(reservationToDelete.ExpiryDate)
                && e.CreatedDate.Equals(reservationToDelete.CreatedDate)
                && e.EmployerDeleted.Equals(command.EmployerDeleted)
                )), Times.Once);
        }
    }
}