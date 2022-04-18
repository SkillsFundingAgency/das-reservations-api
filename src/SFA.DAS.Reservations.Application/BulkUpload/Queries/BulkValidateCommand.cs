using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.BulkUpload.Queries
{
    public class BulkValidateCommand : IRequest<BulkValidationResults>
    {
        public List<BulkValidateRequest> Requests { get; set; }
    }

    public class BulkValidateRequest
    {
        public DateTime? StartDate { get; set; }
        public string CourseId { get; set; }
        public uint? ProviderId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid? UserId { get; set; }
        public int RowNumber { get; set; }
    }
}
