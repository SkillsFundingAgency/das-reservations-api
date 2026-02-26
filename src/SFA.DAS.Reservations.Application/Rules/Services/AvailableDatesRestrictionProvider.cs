using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Types;

namespace SFA.DAS.Reservations.Application.Rules.Services;

public class AvailableDatesRestrictionProvider : IAvailableDatesRestrictionProvider
{
    public bool IncludePreviousMonth(LearningType? learningType) =>
        learningType != LearningType.ApprenticeshipUnit;
}