using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingProviderPermissionsFromRepository
    {
        [Test]
        public async Task Then_The_Permissions_Are_Returned_For_The_Provider()
        {
            //Arrange
            var ukPrn = 10043U;
            var dataContext = new Mock<IReservationsDataContext>();
            dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(new List<ProviderPermission>
            {
                new ProviderPermission
                {
                    UkPrn = ukPrn
                },
                new ProviderPermission
                {
                    UkPrn = ukPrn
                },
                new ProviderPermission
                {
                    UkPrn = 4587
                }
            });
            var repository = new ProviderPermissionRepository(dataContext.Object);

            //Act
            var actual = await repository.GetByProviderId(ukPrn);
            
            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }
    }
}