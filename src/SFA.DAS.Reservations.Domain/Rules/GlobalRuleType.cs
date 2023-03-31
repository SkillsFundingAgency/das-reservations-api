namespace SFA.DAS.Reservations.Domain.Rules
{
    public enum GlobalRuleType
    {
        None = 0,
        FundingPaused = 1,
        ReservationLimit = 2,
        DynamicPause = 3
    }
}