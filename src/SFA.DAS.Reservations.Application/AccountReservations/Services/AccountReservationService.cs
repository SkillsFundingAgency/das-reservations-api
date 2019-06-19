using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
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

        public AccountReservationService(IReservationRepository reservationRepository, IRuleRepository ruleRepository, IOptions<ReservationsConfiguration> options)
        {
            _reservationRepository = reservationRepository;
            _ruleRepository = ruleRepository;
            _options = options;
        }

        public async Task<IList<Reservation>> GetAccountReservations(long accountId)
        {
            var result = await _reservationRepository.GetAccountReservations(accountId);

            var reservations = result
                .Select(MapReservation)
                .ToList();

            return reservations;
        }

        public async Task<Reservation> GetReservation(Guid id)
        {
            var reservation = await _reservationRepository.GetById(id);

            return reservation == null ? null : MapReservation(reservation);
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
                command.AccountLegalEntityId);

            var entity = await _reservationRepository.CreateAccountReservation(MapReservation(reservation));
            var result = MapReservation(entity);

            return result;
        }

        public async Task DeleteReservation(Guid reservationId)
        {
            await _reservationRepository.DeleteAccountReservation(reservationId);
        }

        public Task<IEnumerable<Guid>> BulkCreateAccountReservation(uint reservationCount)
        {
            throw new NotImplementedException();
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
                reservation.AccountLegalEntityName
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
                AccountLegalEntityName = reservation.AccountLegalEntityName
            };
        }
    }
}