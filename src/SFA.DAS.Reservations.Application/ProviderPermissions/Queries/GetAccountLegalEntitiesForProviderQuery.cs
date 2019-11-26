using MediatR;

namespace SFA.DAS.Reservations.Application.ProviderPermissions.Queries
{
    public class GetAccountLegalEntitiesForProviderQuery : IRequest<GetAccountLegalEntitiesForProviderResponse>
    {
        public uint ProviderId { get ; set ; }
    }
}