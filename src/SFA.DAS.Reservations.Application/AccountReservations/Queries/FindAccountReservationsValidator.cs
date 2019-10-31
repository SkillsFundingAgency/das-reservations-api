﻿using System;
using System.Threading.Tasks;
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

            if (string.IsNullOrEmpty(item.SearchTerm))
            {
                validationResult.AddError(nameof(item.SearchTerm));
            }

            return Task.FromResult(validationResult);
        }
    }
}