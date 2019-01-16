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
                .Select(async reservation => await MapReservation(reservation))
                .Select(t => t.Result)
                .ToList();

            return reservations;
        }

        public async Task<long> CreateAccountReservation(Reservation reservation)
        {
            reservation.ExpiryDate = reservation.StartDate.AddMonths(_options.Value.ExpiryPeriodInMonths);
            reservation.Status = ReservationStatus.Pending;
            var result = await _reservationRepository.CreateAccountReservation(MapReservation(reservation));

            return result;
        }

        private async Task<Reservation> MapReservation(Domain.Entities.Reservation reservation)
        {
            var mapReservation = new Reservation(_ruleRepository)
            {
                Id = reservation.Id,
                ExpiryDate = reservation.ExpiryDate,
                CreatedDate = reservation.CreatedDate,
                StartDate = reservation.StartDate,
                AccountId = reservation.AccountId,
                ApprenticeId = reservation.ApprenticeId,
                VacancyId = reservation.VacancyId,
                IsLevyAccount = reservation.IsLevyAccount,
                Status = (ReservationStatus)reservation.Status
            };
            await mapReservation.GetReservationRules();
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
                ApprenticeId = reservation.ApprenticeId,
                VacancyId = reservation.VacancyId,
                Status = (short)reservation.Status
            };
        }
    }
}