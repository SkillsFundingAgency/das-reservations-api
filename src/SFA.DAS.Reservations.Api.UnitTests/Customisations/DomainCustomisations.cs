using AutoFixture;
using AutoFixture.AutoMoq;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Customisations
{
    public class DomainCustomisations : CompositeCustomization
    {
        public DomainCustomisations() : base(
            new AutoMoqCustomization { ConfigureMembers = true })
        {
        }
    }
}
