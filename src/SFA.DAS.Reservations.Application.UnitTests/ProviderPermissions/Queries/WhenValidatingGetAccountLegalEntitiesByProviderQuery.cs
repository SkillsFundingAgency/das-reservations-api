using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermissions.Queries
{
    public class WhenValidatingGetAccountLegalEntitiesByProviderQuery
    {
        private GetAccountLegalEntitiesForProviderValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountLegalEntitiesForProviderValidator();
        }
        
        [Test]
        public async Task Then_Not_Valid_Is_Returned_If_The_Provider_Id_Is_Not_Supplied()
        {
            //Arrange
            var request = new GetAccountLegalEntitiesForProviderQuery
            {
                ProviderId = 0
            };
            
            //Act
            var actual = await _validator.ValidateAsync(request);
            
            //Assert
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Should().ContainKey(nameof(request.ProviderId));
        }

        [Test]
        public async Task Then_Is_Valid_If_All_Params_Are_Supplied()
        {
            //Arrange
            var request = new GetAccountLegalEntitiesForProviderQuery
            {
                ProviderId = 10
            };

            //Act
            var actual = await _validator.ValidateAsync(request);
            
            //Assert
            actual.IsValid().Should().BeTrue();
        }
    }
}