using AutoFixture;
using AutoFixture.NUnit3;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Customisations
{
    public class DomainAutoDataAttribute : AutoDataAttribute
    {
        public DomainAutoDataAttribute()
            : base(() =>
            {
                var fixture = new Fixture();

                fixture
                    .Customize(new DomainCustomisations())
                    .Customize<BindingInfo>(c => c.OmitAutoProperties());

                return fixture;
            })
        { }

    }
}
