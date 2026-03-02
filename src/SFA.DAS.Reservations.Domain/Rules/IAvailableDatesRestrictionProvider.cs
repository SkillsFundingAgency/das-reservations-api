using SFA.DAS.Reservations.Domain.Types;

namespace SFA.DAS.Reservations.Domain.Rules;

public interface IAvailableDatesRestrictionProvider
{
    bool IncludePreviousMonth(LearningType? learningType);
}