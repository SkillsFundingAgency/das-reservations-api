using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsValidator : IValidator<FindAccountReservationsQuery>
    {
        public Task<ValidationResult> ValidateAsync(FindAccountReservationsQuery item)
        {
            throw new NotImplementedException();
        }
    }
}
