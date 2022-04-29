using AutoFixture;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.BulkUpload.Queries;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.BulkCreateReservationsWithNonLevy
{
    public class WhenCreatingANewReservation
    {
        private BulkCreateReservationsCommand _command;
        private BulkCreateReservationsCommandHandler _handler;
        private BulkCreateAccountReservationsResult _levyResult;
        private CreateAccountReservationResult _nonLevyResult;
        private CancellationToken _cancellationToken;
        private Mock<IMediator> _mediator;
        private Mock<IAccountLegalEntitiesService> _accountLegalEntitiesService;
        private Fixture fixture;

        [SetUp]
        public void Arrange()
        {
             fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                    .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _cancellationToken = new CancellationToken();
            _mediator = new Mock<IMediator>();
            _levyResult = fixture.Create<BulkCreateAccountReservationsResult>();
            _nonLevyResult = fixture.Create<CreateAccountReservationResult>();

            _command = fixture.Create<BulkCreateReservationsCommand>();

            _mediator.Setup(x => x.Send(It.IsAny<BulkCreateAccountReservationsCommand>(), _cancellationToken)).ReturnsAsync(_levyResult);
            _mediator.Setup(x => x.Send(It.IsAny<CreateAccountReservationCommand>(), _cancellationToken)).ReturnsAsync(_nonLevyResult);

            _accountLegalEntitiesService = new Mock<IAccountLegalEntitiesService>();
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>())).ReturnsAsync((long accountLegalEntityId) =>
            {
                var accountLegalEntity = fixture.Build<AccountLegalEntity>().
                         With(x => x.AccountLegalEntityId, accountLegalEntityId).Create();
                return accountLegalEntity;
            });

            _mediator.Setup(x => x.Send(It.IsAny<BulkValidateCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => new BulkValidationResults());

            _handler = new BulkCreateReservationsCommandHandler(_mediator.Object, _accountLegalEntitiesService.Object);
        }

        [Test]
        public async Task Then_Levy_Reservations_Are_Created()
        {
            //Arrange
            _command.Reservations.ForEach(x => { x.AccountLegalEntityId = 1; x.TransferSenderAccountId = null; });
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>())).ReturnsAsync((long accountLegalEntityId) =>
                                    new AccountLegalEntity(Guid.NewGuid(), 1, "account legal entity name", 2, accountLegalEntityId, true, true));

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
            _nonLevyResult.Rule = null;
            _nonLevyResult.AgreementSigned = true;
            _command.Reservations.ForEach(x => { x.AccountLegalEntityId = 1; x.TransferSenderAccountId = null; });
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>())).ReturnsAsync((long accountLegalEntityId) =>
                                     new AccountLegalEntity(Guid.NewGuid(), 1, "account legal entity name", 2, accountLegalEntityId, true, false));

            //Act
            BulkCreateReservationsWithNonLevyResult result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _command.Reservations.ForEach(nonLevyEntity =>
            {
                _mediator.Verify(x => x.Send(It.Is<CreateAccountReservationCommand>(y =>
                    y.AccountLegalEntityId == nonLevyEntity.AccountLegalEntityId&&
                    y.CourseId == nonLevyEntity.CourseId&&
                    y.Id == nonLevyEntity.Id&&
                    y.ProviderId == nonLevyEntity.ProviderId&&
                    y.StartDate == nonLevyEntity.StartDate&&
                    y.TransferSenderAccountId == nonLevyEntity.TransferSenderAccountId&&
                    y.UserId == nonLevyEntity.UserId
                ), _cancellationToken), Times.Once);
                _command.Reservations.All(x => result.BulkCreateResults.Single(y => y.ULN == nonLevyEntity.ULN && y.ReservationId != Guid.Empty) != null);
            });
            
        }

        [Test]
        public async Task Then_Levy_Reservations_Are_Created_For_Non_Levy_Employer_When_TransferId_Is_Not_Null()
        {
            //Arrange
            _nonLevyResult.Rule = null;
            _nonLevyResult.AgreementSigned = true;
            _command.Reservations.ForEach(x => { x.AccountLegalEntityId = 1; x.TransferSenderAccountId = 1; });
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>())).ReturnsAsync((long accountLegalEntityId) =>
                                     new AccountLegalEntity(Guid.NewGuid(), 1, "account legal entity name", 2, accountLegalEntityId, true, false));

            //Act
            BulkCreateReservationsWithNonLevyResult result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<BulkCreateAccountReservationsCommand>(y => y.AccountLegalEntityId == 1 && y.TransferSenderAccountId == 1), _cancellationToken), Times.Once);
            _command.Reservations.All(x => result.BulkCreateResults.Single(y => y.ULN == x.ULN && y.ReservationId != Guid.Empty) != null);
        }
    }
}
