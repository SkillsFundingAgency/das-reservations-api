using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Domain.Rules
{
    public interface IGlobalRuleRepository
    {
        Task<ICollection<Entities.GlobalRule>> GetAll();
        Task<ICollection<Entities.GlobalRule>> FindActive(DateTime dateFrom);
    }
}