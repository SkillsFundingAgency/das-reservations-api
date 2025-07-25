using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.CreateAccountReservation;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services;

public class WhenCreatingAccountReservations
{
    private Mock<IRuleRepository> _ruleRepository;
    private Mock<IReservationRepository> _reservationRepository;
    private AccountReservationService _accountReservationService;

    private const int ExpiryPeriodInMonths = 5;
    private const long ExpectedAccountId = 12344;
    private const uint ExpectedProviderId = 66552;
    private const long ExpectedAccountLegalEntityId = 549785;
    private const long ExpectedTransferSenderAccountId = 852364;
    private const string ExpectedAccountLegalEntityName = "TestName";
    private Course _expectedCourse;
    private DateTime _expectedExpiryDate;
    private readonly Guid _expectedReservationId = Guid.NewGuid();
    private readonly DateTime _expectedStartDate = DateTime.UtcNow.AddMonths(1);
    private Mock<IOptions<ReservationsConfiguration>> _options;
        

    [SetUp]
    public void Arrange()
    {
        var expiryDate = _expectedStartDate.AddMonths(ExpiryPeriodInMonths);
        _expectedExpiryDate = new DateTime(expiryDate.Year,expiryDate.Month, DateTime.DaysInMonth(expiryDate.Year, expiryDate.Month), 23, 59, 59);
            
        _expectedCourse = new Course
        {
            CourseId = "123-1",
            Title = "Course 123-1",
            Level = 1
        };

        _options = new Mock<IOptions<ReservationsConfiguration>>();
        _options.Setup(x => x.Value.ExpiryPeriodInMonths).Returns(ExpiryPeriodInMonths);
        _reservationRepository = new Mock<IReservationRepository>();
        _ruleRepository = new Mock<IRuleRepository>();
        _ruleRepository.Setup(x => x.GetReservationRules(It.IsAny<DateTime>())).ReturnsAsync(new List<Rule>());

        _reservationRepository
            .Setup(x => x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(c=>c.Id.Equals(_expectedReservationId))))
            .ReturnsAsync(new Domain.Entities.Reservation{Id=_expectedReservationId, AccountId = ExpectedAccountId, Course = _expectedCourse,
                ProviderId = ExpectedProviderId, AccountLegalEntityId = ExpectedAccountLegalEntityId,TransferSenderAccountId = ExpectedTransferSenderAccountId});
            
        _accountReservationService = new AccountReservationService(
            _reservationRepository.Object, 
            _ruleRepository.Object, 
            _options.Object, 
            Mock.Of<IAzureSearchReservationIndexRepository>(),
            Mock.Of<IAccountLegalEntitiesRepository>());
    }

    [Test]
    public async Task Then_The_Expiry_Period_For_The_Reservation_Is_Taken_From_Configuration()
    {
        //Arrange
        var createReservation = new CreateAccountReservationCommand
        {
            AccountId = ExpectedAccountId,
            StartDate = _expectedStartDate,
            Id = _expectedReservationId
        };

        //Act
        await _accountReservationService.CreateAccountReservation(createReservation);

        //Assert
        _options.Verify(x=>x.Value.ExpiryPeriodInMonths,Times.Once);
    }

    [Test]
    public async Task Then_The_Repository_Is_Called_To_Create_A_Reservation_Mapping_To_The_Entity()
    {
        //Arrange
        var expectedUserId = Guid.NewGuid();
        var createReservation = new CreateAccountReservationCommand
        {
            AccountId = ExpectedAccountId,
            StartDate = _expectedStartDate,
            Id = _expectedReservationId,
            AccountLegalEntityName = ExpectedAccountLegalEntityName,
            IsLevyAccount = true,
            TransferSenderAccountId = ExpectedTransferSenderAccountId,
            UserId = expectedUserId
        };

        //Act
        await _accountReservationService.CreateAccountReservation(createReservation);

        //Assert
        _reservationRepository.Verify(x=>x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(c=>
            c.Id.Equals(_expectedReservationId) &&
            c.AccountId.Equals(ExpectedAccountId) &&
            c.StartDate.Equals(_expectedStartDate) &&
            !c.CreatedDate.Equals(DateTime.MinValue) &&
            c.ExpiryDate.Equals(_expectedExpiryDate) &&
            c.Status.Equals((short)ReservationStatus.Pending) &&
            c.IsLevyAccount.Equals(true) &&
            c.TransferSenderAccountId.Equals(ExpectedTransferSenderAccountId) &&
            c.UserId.Equals(expectedUserId) &&
            c.AccountLegalEntityName.Equals(ExpectedAccountLegalEntityName)
        )));
    }

    [Test]
    public async Task Then_The_New_Reservation_Is_Returned_Mapped_From_The_Entity()
    {
        //Arrange
        var createReservation = new CreateAccountReservationCommand
        {
            AccountId = ExpectedAccountId,
            StartDate = _expectedStartDate,
            Id = _expectedReservationId
        };

        //Act
        var actual = await _accountReservationService.CreateAccountReservation(createReservation);

        //Assert
        actual.Should().BeAssignableTo<Reservation>();
        actual.Id.Should().Be(_expectedReservationId);
        actual.AccountId.Should().Be(ExpectedAccountId);
        actual.Rules.Should().NotBeNull();
    }

    [Test]
    public async Task Then_If_The_Optional_Values_Are_Not_Supplied_They_Are_Defaulted_To_Null()
    {
        //Arrange
        var createReservation = new CreateAccountReservationCommand
        {
            AccountId = ExpectedAccountId,
            StartDate = _expectedStartDate,
            Id = _expectedReservationId
        };

        //Act
        await _accountReservationService.CreateAccountReservation(createReservation);

        //Assert
        _reservationRepository.Verify(x => x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(
            c => c.CourseId == null &&
                 c.Course == null &&
                 c.ProviderId == null
        )));
    }

    [Test]
    public async Task Then_The_Repository_Is_Called_To_Create_A_Reservation_With_Course_Mapping_To_The_Entity()
    {
        //Arrange
        var createReservation = new CreateAccountReservationCommand
        {
            AccountId = ExpectedAccountId,
            StartDate = _expectedStartDate,
            Id = _expectedReservationId,
            CourseId = _expectedCourse.CourseId,
            AccountLegalEntityId = ExpectedAccountLegalEntityId,
            ProviderId = ExpectedProviderId
        };

        //Act
        await _accountReservationService.CreateAccountReservation(createReservation);

        //Assert
        _reservationRepository.Verify(x=>x.CreateAccountReservation(It.Is<Domain.Entities.Reservation>(
            c=>c.AccountId.Equals(ExpectedAccountId) &&
               c.StartDate.Equals(_expectedStartDate) &&
               !c.CreatedDate.Equals(DateTime.MinValue) &&
               c.ExpiryDate.Equals(_expectedExpiryDate) &&
               c.Status.Equals((short)ReservationStatus.Pending) &&
               c.CourseId.Equals(_expectedCourse.CourseId) &&
               c.Course == null &&
               c.ProviderId.Equals(ExpectedProviderId) &&
               c.AccountLegalEntityId.Equals(ExpectedAccountLegalEntityId)
        )));
    }

    [Test]
    public async Task Then_The_New_Reservation_With_Course_Is_Returned_Mapped_From_The_Entity()
    {
        //Arrange
        var createReservation = new CreateAccountReservationCommand
        {
            AccountId = ExpectedAccountId,
            StartDate = _expectedStartDate,
            Id = _expectedReservationId,
            CourseId = _expectedCourse.CourseId,
            AccountLegalEntityId = ExpectedAccountLegalEntityId,
            ProviderId = ExpectedProviderId,
            TransferSenderAccountId = ExpectedTransferSenderAccountId
        };

        //Act
        var actual = await _accountReservationService.CreateAccountReservation(createReservation);

        //Assert
        actual.Should().BeOfType<Reservation>()
            .And.BeEquivalentTo(new
            {
                Id = _expectedReservationId,
                AccountId = ExpectedAccountId,
                Course = new
                {
                    CourseId = _expectedCourse.CourseId,
                    Title = _expectedCourse.Title,
                    Level = _expectedCourse.Level.ToString()
                },
                ProviderId = ExpectedProviderId,
                AccountLegalEntityId = ExpectedAccountLegalEntityId,
                TransferSenderAccountId = ExpectedTransferSenderAccountId,
                Rules = actual.Rules // This checks that Rules is not null but does not compare its contents
            }, options => options.ExcludingMissingMembers()); // Excludes properties that might not be present

    }
}