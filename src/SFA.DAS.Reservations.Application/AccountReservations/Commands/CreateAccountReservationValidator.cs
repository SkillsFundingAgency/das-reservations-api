using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands
{
    public class CreateAccountReservationValidator : IValidator<CreateAccountReservationCommand>
    {
        public Task<ValidationResult> ValidateAsync(CreateAccountReservationCommand item)
        {
            throw new NotImplementedException();
        }
    }
}
