using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.BulkUpload.Queries
{
    public class BulkValidationResults
    {
        public ICollection<BulkValidation> ValidationErrors { get; set; } = new List<BulkValidation>();
    }

    public class BulkValidation
    {
        public string Reason { get; set; }

        public int RowNumber { get; set; }
    }
}
