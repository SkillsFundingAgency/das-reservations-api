using TechTalk.SpecFlow;

namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class ValidateReservationSteps : StepsBase
    {
        public ValidateReservationSteps(TestData testData, TestServiceProvider serviceProvider) 
            : base(testData, serviceProvider)
        {
        }
    }
}