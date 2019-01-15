using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Application.AccountReservations.Services
{
    public class AccountReservationService : IAccountReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRuleRepository _ruleRepository;

        public AccountReservationService(IReservationRepository reservationRepository, IRuleRepository ruleRepository)
        {
            _reservationRepository = reservationRepository;
            _ruleRepository = ruleRepository;
        }

        public async Task<IList<Reservation>> GetAccountReservations(long accountId)
        {
            var result = await _reservationRepository.GetAccountReservations(accountId);

            var reservations = result
                                .Select(async reservation => await MapReservation(reservation))
                                .Select(t=>t.Result)
                                .ToList();

            return reservations;
        }

        public async Task<long> CreateAccountReservation(long accountId, DateTime startDate)
        {
            var result = await _reservationRepository.CreateAccountReservation(accountId, startDate);

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
                IsLevyAccount = reservation.IsLevyAccount
            };
            await mapReservation.GetReservationRules();
            return mapReservation;
        }

        private Domain.Entities.Reservation MapReservation(Reservation reservation)
        {
            return new Domain.Entities.Reservation();
        }
    }
}
