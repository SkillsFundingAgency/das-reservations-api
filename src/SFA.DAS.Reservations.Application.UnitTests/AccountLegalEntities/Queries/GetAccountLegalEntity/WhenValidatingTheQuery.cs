using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Queries.GetAccountLegalEntity
{
    public class WhenValidatingTheQuery
    {
        private GetAccountLegalEntityQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountLegalEntityQueryValidator();
        }

        [Test]
        public async Task Then_The_Query_Is_Invalid_If_The_Required_Fields_Are_Not_Passed_And_Validation_Errors_Returned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountLegalEntityQuery());

            //Assert
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Should().ContainValue("Id has not been supplied");
        }

        [Test]
        public async Task Then_The_Query_Is_Valid_If_The_Values_Are_Valid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountLegalEntityQuery { Id = 99432 });

            //Assert
            actual.IsValid().Should().BeTrue();
        }
    }
}
