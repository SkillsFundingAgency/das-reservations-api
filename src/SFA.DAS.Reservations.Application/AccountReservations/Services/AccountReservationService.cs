using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Extensions;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;
using Reservation = SFA.DAS.Reservations.Domain.Reservations.Reservation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Services
{
    public class AccountReservationService : IAccountReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IOptions<ReservationsConfiguration> _options;
        private readonly IReservationIndexRepository _reservationIndexRepository;
        private readonly IAccountLegalEntitiesRepository _accountLegalEntitiesRepository;

        public AccountReservationService(
            IReservationRepository reservationRepository, 
            IRuleRepository ruleRepository,
            IOptions<ReservationsConfiguration> options, 
            IReservationIndexRepository reservationIndexRepository,
            IAccountLegalEntitiesRepository accountLegalEntitiesRepository)
        {
            _reservationRepository = reservationRepository;
            _ruleRepository = ruleRepository;
            _options = options;
            _reservationIndexRepository = reservationIndexRepository;
            _accountLegalEntitiesRepository = accountLegalEntitiesRepository;
        }

        public async Task<IList<Reservation>> GetAccountReservations(long accountId)
        {
            var result = await _reservationRepository.GetAccountReservations(accountId);

            var reservations = result
                .Select(MapReservation)
                .Where(c=>!c.IsLevyAccount)
                .ToList();

            return reservations;
        }

        public async Task<Reservation> GetReservation(Guid id)
        {
            var reservation = await _reservationRepository.GetById(id);

            return reservation == null ? null : MapReservation(reservation);
        }

        public async Task<ReservationSearchResult> FindReservations(
            long providerId, string searchTerm, ushort pageNumber, ushort pageItemCount, SelectedSearchFilters selectedFilters)
        {
            var result = await _reservationIndexRepository.Find(
                providerId, searchTerm, pageNumber, pageItemCount, selectedFilters);

            return new ReservationSearchResult
            {
                Reservations = result.Reservations.Select(r => r.ToReservation()),
                TotalReservations = result.TotalReservations,
                Filters = result.Filters,
                TotalReservationsForProvider = result.TotalReservationsForProvider
            };
        }

        public async Task<Reservation> CreateAccountReservation(IReservationRequest command)
        {
            var reservation = new Reservation(
                command.Id,
                command.AccountId,
                command.StartDate,
                _options.Value.ExpiryPeriodInMonths,
                command.AccountLegalEntityName,
                command.CourseId,
                command.ProviderId,
                command.AccountLegalEntityId,
                command.IsLevyAccount,
                command.TransferSenderAccountId,
                command.UserId);

            var entity = await _reservationRepository.CreateAccountReservation(MapReservation(reservation));
            var result = MapReservation(entity);

            return result;
        }

        public async Task DeleteReservation(Guid reservationId)
        {
            await _reservationRepository.DeleteAccountReservation(reservationId);
        }

        public async Task<IList<Guid>> BulkCreateAccountReservation(uint reservationCount, long accountLegalEntityId,
            long accountId, string accountLegalEntityName, long? transferSenderAccountId)
        {
            var reservations = new List<Domain.Entities.Reservation>();

            for (var i = 0; i < reservationCount; i++)
            {
                reservations.Add(CreateReservation(accountId,accountLegalEntityId, accountLegalEntityName, transferSenderAccountId));
            }

            await _reservationRepository.CreateAccountReservations(reservations);

            return reservations.Select(c=>c.Id).ToList();
        }

        public async Task<Guid> ChangeOfParty(ChangeOfPartyServiceRequest request)
        {
            var existingReservation = await _reservationRepository.GetById(request.ReservationId);
            if (existingReservation == null)
            {
                throw new EntityNotFoundException<Domain.Entities.Reservation>();
            }

            if (existingReservation.Status != (short)ReservationStatus.Confirmed &&
                existingReservation.Status != (short)ReservationStatus.Change)
            {
                throw new ArgumentException(
                    "Reservation cannot be changed due to it's status.",
                    nameof(ChangeOfPartyServiceRequest.ReservationId));
            }

            var newReservation = new Domain.Entities.Reservation
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ClonedReservationId = existingReservation.Id,
                Status = (short)ReservationStatus.Change,
                StartDate = existingReservation.StartDate,
                ExpiryDate = existingReservation.ExpiryDate,
                CourseId = existingReservation.CourseId,
                ProviderId = existingReservation.ProviderId,
                AccountId = existingReservation.AccountId,
                AccountLegalEntityId = existingReservation.AccountLegalEntityId,
                AccountLegalEntityName = existingReservation.AccountLegalEntityName,
                IsLevyAccount = existingReservation.IsLevyAccount,
                TransferSenderAccountId = existingReservation.TransferSenderAccountId,
                UserId = existingReservation.UserId
            };

            if (request.AccountLegalEntityId.HasValue)
            {
                var newAccountLegalEntity = await _accountLegalEntitiesRepository.Get(request.AccountLegalEntityId.Value);

                newReservation.AccountId = newAccountLegalEntity.AccountId;
                newReservation.AccountLegalEntityId = newAccountLegalEntity.AccountLegalEntityId;
                newReservation.AccountLegalEntityName = newAccountLegalEntity.AccountLegalEntityName;
                newReservation.IsLevyAccount = newAccountLegalEntity.Account.IsLevy ? newAccountLegalEntity.Account.IsLevy : existingReservation.IsLevyAccount;
            } 
            else if (request.ProviderId.HasValue)
            {
                newReservation.ProviderId = request.ProviderId;
            }

            await _reservationRepository.CreateAccountReservation(newReservation);
            return newReservation.Id;
        }

        public async Task<int> GetRemainingReservations(long accountId, int totalReservationAllowed)
        {
            var result = await _reservationRepository.GetAccountReservations(accountId);

            var reservations = result
                .Select(MapReservation)
                .Where(c => !c.IsLevyAccount)
                .ToList();

            var usedReservation = reservations.Where(r => r.CreatedDate >= _options.Value.ResetReservationDate).Count();
            return totalReservationAllowed - usedReservation;
        }

        private Domain.Entities.Reservation CreateReservation(long accountId, long accountLegalEntityId, string accountLegalEntityName, long? transferSenderAccountId)
        {
            return new Domain.Entities.Reservation
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                AccountId = accountId,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityName = accountLegalEntityName,
                IsLevyAccount = true,
                TransferSenderAccountId = transferSenderAccountId
            };
        }

        private Reservation MapReservation(Domain.Entities.Reservation reservation)
        {
            var mapReservation = new Reservation(_ruleRepository.GetReservationRules,
                reservation.Id,
                reservation.AccountId,
                reservation.IsLevyAccount,
                reservation.CreatedDate,
                reservation.StartDate,
                reservation.ExpiryDate,
                (ReservationStatus)reservation.Status,
                reservation.Course,
                reservation.ProviderId,
                reservation.AccountLegalEntityId,
                reservation.AccountLegalEntityName,
                reservation.TransferSenderAccountId,
                reservation.UserId
            );
            return mapReservation;
        }

        private Domain.Entities.Reservation MapReservation(Reservation reservation)
        {
            return new Domain.Entities.Reservation
            {
                Id = reservation.Id,
                ExpiryDate = reservation.ExpiryDate,
                AccountId = reservation.AccountId,
                CreatedDate = reservation.CreatedDate,
                IsLevyAccount = reservation.IsLevyAccount,
                StartDate = reservation.StartDate,
                Status = (short)reservation.Status,
                CourseId = reservation.CourseId,
                AccountLegalEntityId = reservation.AccountLegalEntityId,
                ProviderId = reservation.ProviderId,
                AccountLegalEntityName = reservation.AccountLegalEntityName,
                TransferSenderAccountId = reservation.TransferSenderAccountId,
                UserId = reservation.UserId
            };
        }
    }
}