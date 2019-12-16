using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Reservations.Api.AcceptanceTests
{
    public class TestResults
    {
        public IEnumerable<Domain.AccountLegalEntities.AccountLegalEntity> AccountLegalEntities { get; set; }
        public IActionResult Result { get ; set ; }
    }
}
