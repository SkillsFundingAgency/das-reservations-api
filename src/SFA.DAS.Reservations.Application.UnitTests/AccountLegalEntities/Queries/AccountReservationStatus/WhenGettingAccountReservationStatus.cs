using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Exceptions;
using Entities = SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Queries.AccountReservationStatus
{
    [TestFixture]
    public class WhenGettingAccountReservationStatus
    {
        [Test, MoqAutoData]
        public void And_Fails_Validation_Then_Throws_ValidationException(
            GetAccountReservationStatusQuery query,
            string propertyName,
            ValidationResult validationResult,
            [Frozen] Mock<IValidator<GetAccountReservationStatusQuery>> mockValidator,
            GetAccountReservationStatusQueryHandler handler)
        {
            validationResult.AddError(propertyName);
            mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<GetAccountReservationStatusQuery>()))
                .ReturnsAsync(validationResult);

            var act = async () => await handler.Handle(query, CancellationToken.None);

            act.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"*{propertyName}*");
        }

        [Test, MoqAutoData]
        public void And_No_LegalEntities_For_Account_Then_Throws_EntityNotFoundException(
            GetAccountReservationStatusQuery query,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            GetAccountReservationStatusQueryHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetAccountLegalEntities(It.IsAny<long>()))
                .ReturnsAsync(new List<AccountLegalEntity>());
            mockAccountsService.Setup(x => x.GetAccount(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Account.Account(new Entities.Account(), 100));

            var act = async () => await handler.Handle(query, CancellationToken.None);

            act.Should().ThrowAsync<EntityNotFoundException<Entities.AccountLegalEntity>>();
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_Account_Details_From_Service(
            GetAccountReservationStatusQuery query,
            List<AccountLegalEntity> accountLegalEntities,
            int maxReservations,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            GetAccountReservationStatusQueryHandler handler)
        {
            var accountDetails = new Entities.Account();
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetAccountLegalEntities(It.IsAny<long>()))
                .ReturnsAsync(accountLegalEntities);
            mockAccountsService.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(new Domain.Account.Account(accountDetails, maxReservations));

            var result = await handler.Handle(query, CancellationToken.None);

            var accountLegalEntity = accountLegalEntities[0];
            result.CanAutoCreateReservations.Should().Be(accountLegalEntity.IsLevy);
            result.AccountLegalEntityAgreementStatus[accountLegalEntity.AccountLegalEntityId].Should().Be(accountLegalEntity.AgreementSigned);
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_Reservations_Limit_Reached_From_Service(
            GetAccountReservationStatusQuery query,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IGlobalRulesService> mockGlobalRulesService,
            GetAccountReservationStatusQueryHandler handler)
        {
            var accountDetails = new Entities.Account();
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetAccountLegalEntities(It.IsAny<long>()))
                .ReturnsAsync(accountLegalEntities);
            mockAccountsService.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(new Domain.Account.Account(accountDetails, 100));

            mockGlobalRulesService
                .Setup(x => x.HasReachedReservationLimit(query.AccountId, accountLegalEntities[0].IsLevy))
                .ReturnsAsync(true);

            var result = await handler.Handle(query, CancellationToken.None);

            result.HasReachedReservationsLimit.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_Gets_Calculates_Reservations_Pending_Status_From_Service(
            GetAccountReservationStatusQuery query,
            List<AccountLegalEntity> accountLegalEntities,
            List<Reservation> reservations,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            [Frozen] Mock<IAccountsService> mockAccountsService,
            [Frozen] Mock<IAccountReservationService> mockAccountReservationsService,
            [Frozen] Mock<IGlobalRulesService> mockGlobalRulesService,
            GetAccountReservationStatusQueryHandler handler)
        {
            var accountDetails = new Entities.Account();
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetAccountLegalEntities(It.IsAny<long>()))
                .ReturnsAsync(accountLegalEntities);
            mockAccountsService.Setup(x => x.GetAccount(It.IsAny<long>())).ReturnsAsync(new Domain.Account.Account(accountDetails, 10));
            mockAccountReservationsService.Setup(x => x.GetAccountReservations(It.IsAny<long>())).ReturnsAsync(reservations);

            mockGlobalRulesService
                .Setup(x => x.HasReachedReservationLimit(query.AccountId, accountLegalEntities[0].IsLevy))
                .ReturnsAsync(true);

            var result = await handler.Handle(query, CancellationToken.None);

            result.HasPendingReservations.Should().Be(reservations.Count(x => !x.IsExpired && x.Status == ReservationStatus.Pending) > 0);
        }
    }
}