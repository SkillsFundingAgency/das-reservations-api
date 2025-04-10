using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.Account.Queries.GetAccount
{
    public class GetAccountQueryHandler(IAccountsService service, IValidator<GetAccountQuery> validator)
        : IRequestHandler<GetAccountQuery, GetAccountResult>
    {
        public async Task<GetAccountResult> Handle(GetAccountQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid())
            {
                throw new ArgumentException("The following parameters have failed validation", validationResult.ValidationDictionary.Select(c => c.Key).Aggregate((item1, item2) => item1 + ", " + item2));
            }

            var result = await service.GetAccount(request.Id);

            if (result == null)
            {
                throw new EntityNotFoundException<Domain.Entities.Account>();
            }
            
            return new GetAccountResult{Account = result}; 
        }
    }
}