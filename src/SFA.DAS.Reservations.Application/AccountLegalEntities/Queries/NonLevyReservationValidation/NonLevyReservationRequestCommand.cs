using MediatR;
using SFA.DAS.Reservations.Domain.Reservations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.BulkValidate
{
    public class NonLevyReservationRequestCommand : IRequest<BulkValidationResults> 
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public BulkValidateRequest Request { get; set; }
    }

    //public class BulkValidateRequest : IReservationRequest
    //{
    //    public long AccountId { get; set; }
    //    public DateTime? StartDate { get; set; }
    //    public string CourseId { get; set; }
    //    public uint? ProviderId { get; set; }
    //    public long AccountLegalEntityId { get; set; }
    //    public string AccountLegalEntityName { get; set; }
    //    public bool IsLevyAccount { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public long? TransferSenderAccountId { get; set; }
    //    public Guid? UserId { get; set; }

    //    public Guid Id => throw new NotImplementedException();
    //}
}

