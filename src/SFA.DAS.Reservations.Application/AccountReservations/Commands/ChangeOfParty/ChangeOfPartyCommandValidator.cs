using System;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty
{
    public class ChangeOfPartyCommandValidator: IValidator<ChangeOfPartyCommand>
    {
        public Task<ValidationResult> ValidateAsync(ChangeOfPartyCommand item)
        {
            var validationResult = new ValidationResult();

            if (item.ReservationId == Guid.Empty)
            {
                validationResult.AddError(nameof(item.ReservationId));
            }

            if (!item.AccountLegalEntityId.HasValue && !item.ProviderId.HasValue)
            {
                validationResult.AddError(nameof(item.AccountLegalEntityId));
            }

            if (item.AccountLegalEntityId.HasValue && item.ProviderId.HasValue)
            {
                validationResult.AddError(nameof(item.ProviderId));
            }

            return Task.FromResult(validationResult);
        }
    }
}