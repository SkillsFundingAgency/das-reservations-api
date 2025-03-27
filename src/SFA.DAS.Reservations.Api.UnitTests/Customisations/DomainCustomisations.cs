using AutoFixture;
using AutoFixture.AutoMoq;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Customisations
{
    public class DomainCustomisations() : CompositeCustomization(new AutoMoqCustomization { ConfigureMembers = true });
}
