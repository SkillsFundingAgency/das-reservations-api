using AutoFixture;
using AutoFixture.Kernel;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.BulkUpload.Queries;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Application.UnitTests.BulkUpload.Queries
{
    public class WhenValidatingBulkUpload
    {
        private BulkValidateCommandHandler _handler;
        private BulkValidateCommand _command;
        private CancellationToken _cancellationToken;
        private Mock<IAccountsService> _accountService;
        private Domain.Account.Account _account;
        private Mock<IAccountReservationService> _accountReservationService;
        private Mock<IAccountLegalEntitiesService> _accountLegalEntitiesService;
        private Mock<IMediator> _mediator;
        private int _remainingReservations;
        private AccountLegalEntity _accountLegalEntity;
        private GetAvailableDatesResult _getAvailableDatesResult;
        private GetAccountRulesResult _getAccountRulesResult;
        private GetRulesResult _getRulesResult;

        [SetUp]
        public void Setup()
        {
            var fixure = new Fixture();
            fixure.Customizations.Add(new BulkValidateRequestSpecimenBuilder());
            _remainingReservations = 20;
            _command = fixure.Create<BulkValidateCommand>();
            _cancellationToken = new CancellationToken();

            _getAvailableDatesResult = new GetAvailableDatesResult()
            {
                AvailableDates = new List<AvailableDateStartWindow>()
                {
                    new AvailableDateStartWindow { StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = DateTime.UtcNow.AddMonths(4) },
                    new AvailableDateStartWindow { StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(5) },
                    new AvailableDateStartWindow { StartDate = DateTime.UtcNow.AddMonths(1), EndDate = DateTime.UtcNow.AddMonths(6) }
                }
            };
            _getAccountRulesResult = new GetAccountRulesResult();
            _getRulesResult = new GetRulesResult();
            
            
            _accountLegalEntity = new AccountLegalEntity(Guid.NewGuid(), 1, "Legal entity name", 1, 1, true, false);
            _account = new Domain.Account.Account(1, false, "Legal Entity name", 2);

            _accountService = new Mock<IAccountsService>();
            _accountService.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(() => _account);

            _accountReservationService = new Mock<IAccountReservationService>();
            _accountReservationService.Setup(x => x.GetRemainingReservations(It.IsAny<long>(), It.IsAny<int>())).ReturnsAsync(() => _remainingReservations);

            _accountLegalEntitiesService = new Mock<IAccountLegalEntitiesService>();
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>())).ReturnsAsync(() => _accountLegalEntity);

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetAvailableDatesQuery>(), _cancellationToken)).ReturnsAsync(() => _getAvailableDatesResult);
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountRulesQuery>(), _cancellationToken)).ReturnsAsync(() => _getAccountRulesResult);
            _mediator.Setup(x => x.Send(It.IsAny<GetRulesQuery>(), _cancellationToken)).ReturnsAsync(() => _getRulesResult);

            _handler = new BulkValidateCommandHandler(_accountReservationService.Object, Mock.Of<IGlobalRulesService>(), _accountLegalEntitiesService.Object, _accountService.Object, _mediator.Object);
        }

        [Test]
        public async Task Fails_Validation_when_Apprenticeships_Exceeds_Remaining_Reservations()
        {
            //Setup
            _remainingReservations = 2;

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(1, result.ValidationErrors.Count);
            Assert.AreEqual("The employer has reached their reservations limit. Contact the employer.", result.ValidationErrors.First().Reason);
        }

        [Test]
        public async Task Fails_Validation_when_Apprenticeshp_StartDate_Is_Before_Reservation_window()
        {
            //Setup
            _command.Requests.First().StartDate = DateTime.UtcNow.AddMonths(-2);

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(1, result.ValidationErrors.Count);
            Assert.AreEqual($"The start for this learner cannot be before {_getAvailableDatesResult.AvailableDates.Select(x => x.StartDate).Min():dd/MM/yyyy} (first month of the window). You cannot backdate reserve funding.", result.ValidationErrors.First().Reason);
        }

        [Test]
        public async Task Fails_Validation_when_Apprenticeshp_StartDate_Is_After_Reservation_window()
        {
            //Setup
            _command.Requests.First().StartDate = DateTime.UtcNow.AddMonths(2);

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            var currentDate = DateTime.UtcNow;
            var maxDate = _getAvailableDatesResult.AvailableDates.Select(x => x.StartDate).Max();
            var monthsInAdvance = ((currentDate.Year - maxDate.Year) * 12) + currentDate.Month - maxDate.Month;
            Assert.AreEqual(1, result.ValidationErrors.Count);
            Assert.AreEqual($"The start for this learner cannot be after {_getAvailableDatesResult.AvailableDates.Select(x => x.StartDate).Max():dd/MM/yyyy} (last month of the window) You cannot reserve funding more than {monthsInAdvance} months in advance.", result.ValidationErrors.First().Reason);
        }
    }

    public class BulkValidateRequestSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(BulkValidateRequest))
            {
                var fixture = new Fixture();
                var startDate = fixture.Create<DateTime>();
                var endDate = fixture.Create<DateTime>();
                var dob = fixture.Create<DateTime>();
                return fixture.Build<BulkValidateRequest>()
                    .With(x => x.StartDate, DateTime.UtcNow)
                    .With(x => x.AccountLegalEntityId, 1)
                    .Create();
            }

            return new NoSpecimen();
        }
    }
}
