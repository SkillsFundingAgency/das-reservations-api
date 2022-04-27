using SFA.DAS.Reservations.Application.BulkUpload.Queries;
using System;

namespace SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateReservationsWithNonLevy
{
    public class BulkCreateReservations
    {
        public Guid Id { get; set; }
        public DateTime? StartDate { get; set; }
        public string CourseId { get; set; }
        public uint? ProviderId { get; set; }
        public long? AccountLegalEntityId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid? UserId { get; set; }
        public string ULN { get; set; }
        public int RowNumber { get; set; }


        public static implicit operator BulkValidateRequest(BulkCreateReservations response)
        {
            var legalEntityId = response.AccountLegalEntityId.HasValue ? response.AccountLegalEntityId.Value : 0;
         
            return new BulkValidateRequest
            {
                CourseId = response.CourseId,
                AccountLegalEntityId = legalEntityId,
                ProviderId = (uint)response.ProviderId,
                RowNumber = response.RowNumber,
                StartDate = response.StartDate,
                TransferSenderAccountId = response.TransferSenderAccountId,
                UserId = response.UserId,
                CreatedDate = DateTime.UtcNow
            };
        }
    }
}
