using MediatR;

namespace SFA.DAS.Reservations.Application.Account.Queries.GetAccount
{
    public class GetAccountQuery : IRequest<GetAccountResult>
    {
        public long Id { get; set; }
    }
}