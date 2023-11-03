using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Kernel;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.BulkUpload.Queries;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

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
        private Mock<IOptions<ReservationsConfiguration>> _configuration;
        private Mock<ICurrentDateTime> _currentDateTime;
        private readonly DateTime _referenceDate = DateTime.UtcNow;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            fixture.Customizations.Add(new BulkValidateRequestSpecimenBuilder());
            _remainingReservations = 20;
            _command = fixture.Create<BulkValidateCommand>();
            _cancellationToken = new CancellationToken();

            _getAvailableDatesResult = new GetAvailableDatesResult
            {
                AvailableDates = new List<AvailableDateStartWindow>
                {
                    new() { StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = DateTime.UtcNow.AddMonths(2) },
                    new() { StartDate = _referenceDate, EndDate = _referenceDate.AddMonths(3) },
                    new() { StartDate = _referenceDate.AddMonths(1), EndDate = _referenceDate.AddMonths(4) }
                }
            };

            _getAccountRulesResult = new GetAccountRulesResult();
            _getRulesResult = new GetRulesResult();

            _accountLegalEntity = new AccountLegalEntity(Guid.NewGuid(), 1, "Legal entity name", 1, 1, true, false);
            _account = new Domain.Account.Account(1, false, "Legal Entity name", 2);

            _accountService = new Mock<IAccountsService>();
            _accountService.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(() => _account);

            _accountReservationService = new Mock<IAccountReservationService>();
            _accountReservationService.Setup(x => x.GetRemainingReservations(It.IsAny<long>(), It.IsAny<int>()))
                .ReturnsAsync(() => _remainingReservations);

            _accountLegalEntitiesService = new Mock<IAccountLegalEntitiesService>();
            _accountLegalEntitiesService.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>()))
                .ReturnsAsync(() => _accountLegalEntity);

            _mediator = new Mock<IMediator>();
            
            _mediator.Setup(x => x.Send(It.IsAny<GetAvailableDatesQuery>(), _cancellationToken))
                .ReturnsAsync(() => _getAvailableDatesResult);
            
            _mediator.Setup(x => x.Send(It.IsAny<GetAccountRulesQuery>(), _cancellationToken))
                .ReturnsAsync(() => _getAccountRulesResult);
            
            _mediator.Setup(x => x.Send(It.IsAny<GetRulesQuery>(), _cancellationToken))
                .ReturnsAsync(() => _getRulesResult);

            _configuration = new Mock<IOptions<ReservationsConfiguration>>();
            _configuration.Setup(x => x.Value).Returns(new ReservationsConfiguration { ExpiryPeriodInMonths = 2 });

            _currentDateTime = new Mock<ICurrentDateTime>();
            _currentDateTime.Setup(x => x.GetDate()).Returns(_referenceDate);

            _handler = new BulkValidateCommandHandler(
                _accountReservationService.Object,
                Mock.Of<IGlobalRulesService>(),
                _accountLegalEntitiesService.Object,
                _accountService.Object,
                _mediator.Object,
                _configuration.Object,
                Mock.Of<ILogger<BulkValidateCommandHandler>>(),
                _currentDateTime.Object);
        }

        [Test]
        public async Task Fails_Validation_when_Apprenticeships_Exceeds_Remaining_Reservations()
        {
            //Setup
            _remainingReservations = 2;

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            Assert.AreEqual(3, result.ValidationErrors.Count);
            Assert.AreEqual("The employer has reached their <b>reservations limit</b>. Contact the employer.",
                result.ValidationErrors.First().Reason);
        }

        [Test]
        public async Task Fails_Validation_when_Apprenticeship_StartDate_Is_Before_One_Month_Prior_To_Start_Date_Of_Current_Month()
        {
            //Setup
            _command.Requests.First().StartDate = _referenceDate.AddMonths(-2);

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);

            //Assert
            var previousMonthDate = _referenceDate.AddMonths(-1);
            var firstDateOfPreviousMonth = new DateTime(previousMonthDate.Year, previousMonthDate.Month, 1);

            Assert.AreEqual(1, result.ValidationErrors.Count);
            Assert.AreEqual($"The start date cannot be before {firstDateOfPreviousMonth:dd/MM/yyyy}. You can only backdate a reservation for 1 month.", result.ValidationErrors.First().Reason);
        }
        
        [Test]
        public async Task Fails_Validation_when_Apprenticeship_StartDate_Is_After_Reservation_window()
        {
            //Setup
            _command.Requests.First().StartDate = _referenceDate.AddMonths(5);

            //Act
            var result = await _handler.Handle(_command, _cancellationToken);
            var possibleEndDate = _getAvailableDatesResult.AvailableDates.Select(x => x.StartDate).Max();
            var maxDate = new DateTime(possibleEndDate.Year, possibleEndDate.Month,
                DateTime.DaysInMonth(possibleEndDate.Year, possibleEndDate.Month));

            //Assert
            Assert.AreEqual(1, result.ValidationErrors.Count);
            Assert.AreEqual(
                $"The start for this learner cannot be after {maxDate:dd/MM/yyyy} (last month of the window) You cannot reserve funding more than {_configuration.Object.Value.ExpiryPeriodInMonths} months in advance.",
                result.ValidationErrors.First().Reason);
        }
    }

    public class BulkValidateRequestSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request is Type type && type == typeof(BulkValidateRequest))
            {
                return new Fixture()
                    .Build<BulkValidateRequest>()
                    .With(x => x.StartDate, DateTime.UtcNow)
                    .With(x => x.AccountLegalEntityId, 1)
                    .With(x => x.TransferSenderAccountId, (long?)null)
                    .Create();
            }

            return new NoSpecimen();
        }
    }
}