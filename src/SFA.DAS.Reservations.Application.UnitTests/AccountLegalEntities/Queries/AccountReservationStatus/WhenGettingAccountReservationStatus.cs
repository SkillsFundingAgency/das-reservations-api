using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Exceptions;
using Entities = SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;
using ValidationResult = SFA.DAS.Reservations.Domain.Validation.ValidationResult;

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

            var act = new Func<Task>(async () => await handler.Handle(query, CancellationToken.None));

            act.Should().Throw<ArgumentException>()
                .WithMessage($"*{propertyName}*");
        }

        [Test, MoqAutoData]
        public void And_No_LegalEntities_For_Account_Then_Throws_EntityNotFoundException(
            GetAccountReservationStatusQuery query,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            GetAccountReservationStatusQueryHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            query.TransferSenderAccountId = null;
            mockService
                .Setup(service => service.GetAccountLegalEntities(It.IsAny<long>()))
                .ReturnsAsync(new List<AccountLegalEntity>());

            var act = new Func<Task>(async () => await handler.Handle(query, CancellationToken.None));

            act.Should().Throw<EntityNotFoundException<Entities.AccountLegalEntity>>();
        }

        [Test, MoqAutoData]
        public async Task And_There_Is_A_TransferSenderAccountId_Then_That_Is_Checked_To_See_If_Levy(
            GetAccountReservationStatusQuery query,
            long transferSenderAccountId,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            GetAccountReservationStatusQueryHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            query.TransferSenderAccountId = transferSenderAccountId;
            mockService
                .Setup(service => service.GetAccountLegalEntities(transferSenderAccountId))
                .ReturnsAsync(accountLegalEntities);

            var result = await handler.Handle(query, CancellationToken.None);

            result.CanAutoCreateReservations.Should().Be(accountLegalEntities[0].IsLevy);

        }

        [Test, MoqAutoData]
        public async Task Then_Gets_Account_Details_From_Service(
            GetAccountReservationStatusQuery query,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountLegalEntitiesService> mockService,
            GetAccountReservationStatusQueryHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            query.TransferSenderAccountId = null;
            mockService
                .Setup(service => service.GetAccountLegalEntities(It.IsAny<long>()))
                .ReturnsAsync(accountLegalEntities);

            var result = await handler.Handle(query, CancellationToken.None);

            var accountLegalEntity = accountLegalEntities[0];
            result.CanAutoCreateReservations.Should().Be(accountLegalEntity.IsLevy);
            result.AccountLegalEntityAgreementStatus[accountLegalEntity.AccountLegalEntityId].Should().Be(accountLegalEntity.AgreementSigned);
        }
    }
}