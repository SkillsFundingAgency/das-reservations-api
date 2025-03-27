namespace SFA.DAS.Reservations.Domain.Validation
{
    public class ReservationValidationError(string propertyName, string reason)
    {
        public string PropertyName { get; } = propertyName;
        public string Reason { get;  } = reason;
    }
}
