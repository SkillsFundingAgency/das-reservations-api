using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IGlobalRulesService
    {
        Task<IList<GlobalRule>> GetAllRules();
        Task<IList<GlobalRule>> GetActiveRules(DateTime fromDate);
        Task<GlobalRule> CheckReservationAgainstRules(IReservationRequest request);
        Task<IList<GlobalRule>> GetAccountRules(long accountId);
        Task<bool> HasReachedReservationLimit(long accountId, bool isLevyReservation = false);
    }
}