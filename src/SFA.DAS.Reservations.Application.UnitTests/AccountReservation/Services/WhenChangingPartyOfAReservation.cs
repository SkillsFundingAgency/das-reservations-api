namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenChangingPartyOfAReservation
    {
        
        /*[Test, MoqAutoData]
        public void And_Reservation_Not_Found_Then_Throws_EntityNotFoundException(
            ChangeOfPartyCommand command,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountReservationService> mockService,
            ChangeOfPartyCommandHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetReservation(command.ReservationId))
                .ReturnsAsync((Reservation) null);

            var act = new Func<Task>(async () => await handler.Handle(command, CancellationToken.None));

            act.Should().Throw<EntityNotFoundException<Reservation>>();
        }*/
    }
}