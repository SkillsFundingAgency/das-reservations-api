using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Queries;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Queries
{
    public class WhenValidatingFindReservations
    {
        
        private FindAccountReservationsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new FindAccountReservationsValidator();
        }

        [Test]
        public async Task Then_The_Query_Is_Invalid_If_The_Required_Fields_Are_Not_Passed_And_Validation_Errors_Returned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new FindAccountReservationsQuery());

            //Assert
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Should().ContainValue("ProviderId has not been supplied");
            actual.ValidationDictionary.Should().ContainValue("PageNumber has not been supplied");
            actual.ValidationDictionary.Should().ContainValue("PageItemCount has not been supplied");
        }

        [Test]
        public async Task Then_The_Query_Is_Valid_If_The_Values_Are_Valid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new FindAccountReservationsQuery
            {
                ProviderId = 99432, 
                SearchTerm = "test",
                PageNumber = 1,
                PageItemCount = 50
            });

            //Assert
            actual.IsValid().Should().BeTrue();
        }
    }
}
