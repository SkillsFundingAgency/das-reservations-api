﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.BulkValidate
{
    public class BulkValidationResults
    {
        public ICollection<BulkValidation> ValidationErrors { get; set; }
    }

    public class BulkValidation
    {
        public string Reason { get; set; }

        public bool FileLevelError { get; set; }

        public long ApprenticeshipName { get; set; }
    }
}
