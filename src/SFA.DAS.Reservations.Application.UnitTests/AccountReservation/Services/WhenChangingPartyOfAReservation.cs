﻿using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenChangingPartyOfAReservation
    {
        [Test, MoqAutoData]
        public async Task And_Reservation_Not_Found_Then_Throws_EntityNotFoundException(
            ChangeOfPartyServiceRequest request,
            [Frozen] Mock<IReservationRepository> mockRepository,
            AccountReservationService service)
        {
            mockRepository
                .Setup(repository => repository.GetById(request.ReservationId))
                .ReturnsAsync((Domain.Entities.Reservation) null);

            var act = async () => await service.ChangeOfParty(request);

            await act.Should().ThrowAsync<EntityNotFoundException<Domain.Entities.Reservation>>();
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_Reservation_Completed_Status_Then_Throws_ArgumentException(
            ChangeOfPartyServiceRequest request,
            Domain.Entities.Reservation existingReservation,
            [Frozen] Mock<IReservationRepository> mockRepository,
            AccountReservationService service)
        {
            existingReservation.Status = (short) ReservationStatus.Completed;
            mockRepository
                .Setup(repository => repository.GetById(request.ReservationId))
                .ReturnsAsync(existingReservation);

            var act = async () => await service.ChangeOfParty(request);

            await act.Should().ThrowAsync<ArgumentException>()
                .Where(exception => 
                    exception.ParamName == nameof(ChangeOfPartyServiceRequest.ReservationId) &&
                    exception.Message.StartsWith("Reservation cannot be changed due to it's status."));
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_Reservation_Deleted_Status_Then_Throws_ArgumentException(
            ChangeOfPartyServiceRequest request,
            Domain.Entities.Reservation existingReservation,
            [Frozen] Mock<IReservationRepository> mockRepository,
            AccountReservationService service)
        {
            existingReservation.Status = (short) ReservationStatus.Deleted;
            mockRepository
                .Setup(repository => repository.GetById(request.ReservationId))
                .ReturnsAsync(existingReservation);

            var act = async () => await service.ChangeOfParty(request);

            await act.Should().ThrowAsync<ArgumentException>()
                .Where(exception => 
                    exception.ParamName == nameof(ChangeOfPartyServiceRequest.ReservationId) &&
                    exception.Message.StartsWith("Reservation cannot be changed due to it's status."));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_New_Reservation_Cloned_From_Existing(
            ChangeOfPartyServiceRequest request,
            Domain.Entities.Reservation existingReservation,
            [Frozen] Mock<IReservationRepository> mockRepository,
            AccountReservationService service)
        {
            request.AccountLegalEntityId = null;
            existingReservation.Status = (short) ReservationStatus.Confirmed;
            mockRepository
                .Setup(repository => repository.GetById(request.ReservationId))
                .ReturnsAsync(existingReservation);

            var newReservationId = await service.ChangeOfParty(request);

            newReservationId.Should().NotBeEmpty();
            mockRepository
                .Verify(repository => repository.CreateAccountReservation(
                    It.Is<Domain.Entities.Reservation>(reservation => 
                        reservation.Id == newReservationId &&
                        reservation.Status == (short)ReservationStatus.Change &&
                        reservation.ClonedReservationId == existingReservation.Id &&
                        reservation.AccountId == existingReservation.AccountId &&
                        reservation.AccountLegalEntityId == existingReservation.AccountLegalEntityId &&
                        reservation.AccountLegalEntityName == existingReservation.AccountLegalEntityName &&
                        reservation.ProviderId == request.ProviderId &&
                        reservation.CourseId == existingReservation.CourseId &&
                        reservation.StartDate == existingReservation.StartDate &&
                        reservation.ExpiryDate == existingReservation.ExpiryDate &&
                        reservation.IsLevyAccount == existingReservation.IsLevyAccount &&
                        reservation.TransferSenderAccountId == existingReservation.TransferSenderAccountId)));
        }

        [Test, RecursiveMoqAutoData]
        public async Task And_New_AccountLegalEntityId_Then_Gets_All_Details_For_That_AccountLegalEntity(
            ChangeOfPartyServiceRequest request,
            Domain.Entities.Reservation existingReservation,
            Domain.Entities.AccountLegalEntity newAccountLegalEntity,
            [Frozen] Mock<IReservationRepository> mockRepository,
            [Frozen] Mock<IAccountLegalEntitiesRepository> mockAleRepository,
            AccountReservationService service)
        {
            request.ProviderId = null;
            existingReservation.Status = (short) ReservationStatus.Change;
            existingReservation.IsLevyAccount = false;
            newAccountLegalEntity.Account.IsLevy = true;
            mockRepository
                .Setup(repository => repository.GetById(request.ReservationId))
                .ReturnsAsync(existingReservation);
            mockAleRepository
                .Setup(repository => repository.Get(request.AccountLegalEntityId.Value))
                .ReturnsAsync(newAccountLegalEntity);

            var newReservationId = await service.ChangeOfParty(request);

            newReservationId.Should().NotBeEmpty();
            mockRepository
                .Verify(repository => repository.CreateAccountReservation(
                    It.Is<Domain.Entities.Reservation>(reservation => 
                        reservation.Id == newReservationId &&
                        reservation.Status == (short)ReservationStatus.Change &&
                        reservation.ClonedReservationId == existingReservation.Id &&
                        reservation.AccountId == newAccountLegalEntity.AccountId &&
                        reservation.AccountLegalEntityId == newAccountLegalEntity.AccountLegalEntityId &&
                        reservation.AccountLegalEntityName == newAccountLegalEntity.AccountLegalEntityName &&
                        reservation.ProviderId == existingReservation.ProviderId &&
                        reservation.CourseId == existingReservation.CourseId &&
                        reservation.StartDate == existingReservation.StartDate &&
                        reservation.ExpiryDate == existingReservation.ExpiryDate &&
                        reservation.IsLevyAccount == newAccountLegalEntity.Account.IsLevy)));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_Only_Updates_The_Levy_Flag_If_Going_From_NonLevy_To_Levy(ChangeOfPartyServiceRequest request,
            Domain.Entities.Reservation existingReservation,
            Domain.Entities.AccountLegalEntity newAccountLegalEntity,
            [Frozen] Mock<IReservationRepository> mockRepository,
            [Frozen] Mock<IAccountLegalEntitiesRepository> mockAleRepository,
            AccountReservationService service)
        {
            request.ProviderId = null;
            existingReservation.Status = (short) ReservationStatus.Change;
            existingReservation.IsLevyAccount = true;
            newAccountLegalEntity.Account.IsLevy = false;
            mockRepository
                .Setup(repository => repository.GetById(request.ReservationId))
                .ReturnsAsync(existingReservation);
            mockAleRepository
                .Setup(repository => repository.Get(request.AccountLegalEntityId.Value))
                .ReturnsAsync(newAccountLegalEntity);

            var newReservationId = await service.ChangeOfParty(request);

            newReservationId.Should().NotBeEmpty();
            mockRepository
                .Verify(repository => repository.CreateAccountReservation(
                    It.Is<Domain.Entities.Reservation>(reservation => 
                        reservation.Id == newReservationId &&
                        reservation.Status == (short)ReservationStatus.Change &&
                        reservation.ClonedReservationId == existingReservation.Id &&
                        reservation.AccountId == newAccountLegalEntity.AccountId &&
                        reservation.AccountLegalEntityId == newAccountLegalEntity.AccountLegalEntityId &&
                        reservation.AccountLegalEntityName == newAccountLegalEntity.AccountLegalEntityName &&
                        reservation.ProviderId == existingReservation.ProviderId &&
                        reservation.CourseId == existingReservation.CourseId &&
                        reservation.StartDate == existingReservation.StartDate &&
                        reservation.ExpiryDate == existingReservation.ExpiryDate &&
                        reservation.IsLevyAccount)));
        }
    }
}