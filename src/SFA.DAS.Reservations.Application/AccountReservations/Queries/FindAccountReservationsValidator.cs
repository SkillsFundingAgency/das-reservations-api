﻿using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountReservations.Queries
{
    public class FindAccountReservationsValidator : IValidator<FindAccountReservationsQuery>
    {
        public Task<ValidationResult> ValidateAsync(FindAccountReservationsQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.ProviderId == 0)
            {
                validationResult.AddError(nameof(item.ProviderId));
            }

            if (item.PageNumber == 0)
            {
                validationResult.AddError(nameof(item.PageNumber));
            }

            if (item.PageItemCount == 0)
            {
                validationResult.AddError(nameof(item.PageItemCount));
            }

            return Task.FromResult(validationResult);
        }
    }
}
