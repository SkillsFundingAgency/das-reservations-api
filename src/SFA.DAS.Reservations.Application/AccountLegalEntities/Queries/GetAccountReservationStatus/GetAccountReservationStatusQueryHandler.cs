using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus
{
    public class GetAccountReservationStatusQueryHandler : IRequestHandler<GetAccountReservationStatusQuery, GetAccountReservationStatusResponse>
    {
        private readonly IValidator<GetAccountReservationStatusQuery> _validator;
        private readonly IAccountLegalEntitiesService _accountLegalEntitiesService;

        public GetAccountReservationStatusQueryHandler(
            IValidator<GetAccountReservationStatusQuery> validator,
            IAccountLegalEntitiesService accountLegalEntitiesService)
        {
            _validator = validator;
            _accountLegalEntitiesService = accountLegalEntitiesService;
        }

        public async Task<GetAccountReservationStatusResponse> Handle(GetAccountReservationStatusQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException(
                    "The following parameters have failed validation", 
                    validationResult.ValidationDictionary
                        .Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var accountLegalEntities = await _accountLegalEntitiesService.GetAccountLegalEntities(
                request.TransferSenderAccountId ?? request.AccountId);

            if (accountLegalEntities == null || accountLegalEntities.Count == 0)
            {
                throw new EntityNotFoundException<Domain.Entities.AccountLegalEntity>();
            }

            return new GetAccountReservationStatusResponse
            {
                CanAutoCreateReservations = accountLegalEntities[0].IsLevy
            };
        }
    }
}