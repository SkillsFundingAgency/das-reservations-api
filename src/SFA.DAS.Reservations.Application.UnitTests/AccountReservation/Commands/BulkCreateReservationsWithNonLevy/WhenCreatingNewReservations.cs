using AutoFixture;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.BulkCreateReservationsWithNonLevy
{
    public class WhenCreatingANewReservation
    {
        private BulkCreateReservationsWithNonLevyCommand _command;
        private BulkCreateReservationsWithNonLevyCommandHandler _handler;
        private BulkCreateAccountReservationsResult _levyResult;
        private CreateAccountReservationResult _nonLevyResult;
        private CancellationToken _cancellationToken;
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _cancellationToken = new CancellationToken();
            _mediator = new Mock<IMediator>();
            _levyResult = fixture.Create<BulkCreateAccountReservationsResult>();
            _nonLevyResult = fixture.Create<CreateAccountReservationResult>();

            _command = fixture.Create<BulkCreateReservationsWithNonLevyCommand>();

            _mediator.Setup(x => x.Send(It.IsAny<BulkCreateAccountReservationsCommand>(), _cancellationToken)).ReturnsAsync(_levyResult);
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(), _cancellationToken)).ReturnsAsync(_nonLevyResult);

            _handler = new BulkCreateReservationsWithNonLevyCommandHandler(_mediator.Object);
        }

        [Test]
        public async Task Then_Levy_Reservations_Are_Created()
        {
            //Arrange
            _command.Reservations.ForEach(x => { x.IsLevyAccount = true; x.AccountLegalEntityId = 1; x.TransferSenderAccountId = null; });

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<BulkCreateAccountReservationsCommand>(y => y.AccountLegalEntityId == 1 && y.TransferSenderAccountId == null), _cancellationToken), Times.Once);
            _command.Reservations.All(x => result.BulkCreateResults.Single(y => y.ULN == x.ULN && y.ReservationId != Guid.Empty) != null);
        }


        [Test]
        public async Task Then_Non_Levy_Reservations_Are_Created()
        {
            //Arrange
            _command.Reservations.ForEach(x => { x.IsLevyAccount = false; });
            _nonLevyResult.Rule = null;
            _nonLevyResult.AgreementSigned = true;

            //Act
            BulkCreateReservationsWithNonLevyResult result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _command.Reservations.ForEach(nonLevyEntity =>
            {
                _mediator.Verify(x => x.Send(It.Is<CreateAccountReservationCommand>(y =>

                    y.AccountId == nonLevyEntity.AccountId&&
                    y.AccountLegalEntityId == nonLevyEntity.AccountLegalEntityId&&
                    y.AccountLegalEntityName == nonLevyEntity.AccountLegalEntityName&&
                    y.CourseId == nonLevyEntity.CourseId&&
                    y.CreatedDate == nonLevyEntity.CreatedDate&&
                    y.Id == nonLevyEntity.Id&&
                    y.IsLevyAccount == nonLevyEntity.IsLevyAccount&&
                    y.ProviderId == nonLevyEntity.ProviderId&&
                    y.StartDate == nonLevyEntity.StartDate&&
                    y.TransferSenderAccountId == nonLevyEntity.TransferSenderAccountId&&
                    y.UserId == nonLevyEntity.UserId
                ), _cancellationToken), Times.Once);
                _command.Reservations.All(x => result.BulkCreateResults.Single(y => y.ULN == nonLevyEntity.ULN && y.ReservationId != Guid.Empty) != null);
            });
            
        }
    }
}
