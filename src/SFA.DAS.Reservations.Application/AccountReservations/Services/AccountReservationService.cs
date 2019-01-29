using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.AccountReservations.Services
{
    public class AccountReservationService : IAccountReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IOptions<ReservationConfiguration> _options;

        public AccountReservationService(IReservationRepository reservationRepository, IRuleRepository ruleRepository, IOptions<ReservationConfiguration> options)
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

        public async Task<Reservation> CreateAccountReservation(long accountId, DateTime startDate)
        {
            var reservation = new Reservation(accountId, startDate, _options.Value.ExpiryPeriodInMonths);
            
            var entity = await _reservationRepository.CreateAccountReservation(MapReservation(reservation));
            var result = MapReservation(entity);

            return result;
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
                (ReservationStatus)reservation.Status
            );
            return mapReservation;
        }

        private Domain.Entities.Reservation MapReservation(Reservation reservation)
        {
            return new Domain.Entities.Reservation
            {
                ExpiryDate = reservation.ExpiryDate,
                AccountId = reservation.AccountId,
                CreatedDate = reservation.CreatedDate,
                IsLevyAccount = reservation.IsLevyAccount,
                StartDate = reservation.StartDate,
                Status = (short)reservation.Status
            };
        }
    }
}