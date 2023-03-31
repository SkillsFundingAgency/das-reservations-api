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
                CreatePermission(ukPrn),
                CreatePermission(ukPrn),
                CreatePermission(12345)
            });
            var repository = new ProviderPermissionRepository(dataContext.Object);

            //Act
            var actual = await repository.GetAllowedNonLevyPermissionsForProvider(ukPrn);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task Then_The_Non_Levy_LegalEntities_Permissions_Are_Returned_For_The_Provider()
        {
            //Arrange
            var ukPrn = 10043U;
            var dataContext = new Mock<IReservationsDataContext>();
            dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(new List<ProviderPermission>
            {
                CreatePermission(ukPrn),
                CreatePermission(ukPrn),
                CreatePermission(ukPrn, isLevy: true)
            });
            var repository = new ProviderPermissionRepository(dataContext.Object);

            //Act
            var actual = await repository.GetAllowedNonLevyPermissionsForProvider(ukPrn);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task Then_The_Non_Levy_LegalEntities_With_AllowCohort_Permissions_Are_Returned_For_The_Provider()
        {
            //Arrange
            var ukPrn = 10043U;
            var dataContext = new Mock<IReservationsDataContext>();
            dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(new List<ProviderPermission>
            {
                CreatePermission(ukPrn),
                CreatePermission(ukPrn),
                CreatePermission(ukPrn, canCreateCohort: false)
            });
            var repository = new ProviderPermissionRepository(dataContext.Object);

            //Act
            var actual = await repository.GetAllowedNonLevyPermissionsForProvider(ukPrn);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task Then_Permissions_With_No_Associated_Account_Are_Not_Returned()
        {
            //Arrange
            var ukPrn = 10043U;
            var dataContext = new Mock<IReservationsDataContext>();
            dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(new List<ProviderPermission>
            {
                CreatePermission(ukPrn),
                CreatePermission(ukPrn),
                CreatePermission(ukPrn, setAccount: false)
            });
            var repository = new ProviderPermissionRepository(dataContext.Object);

            //Act
            var actual = await repository.GetAllowedNonLevyPermissionsForProvider(ukPrn);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task Then_Permissions_With_No_Associated_AccountLegalEntity_Are_Not_Returned()
        {
            //Arrange
            var ukPrn = 10043U;
            var dataContext = new Mock<IReservationsDataContext>();
            dataContext.Setup(x => x.ProviderPermissions).ReturnsDbSet(new List<ProviderPermission>
            {
                CreatePermission(ukPrn),
                CreatePermission(ukPrn),
                CreatePermission(ukPrn, setAccount: false)
            });
            var repository = new ProviderPermissionRepository(dataContext.Object);

            //Act
            var actual = await repository.GetAllowedNonLevyPermissionsForProvider(ukPrn);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Count);
        }

        private ProviderPermission CreatePermission(uint ukPrn, bool setAccount = true, bool isLevy = false, bool setAccountLegalEntity = true, bool canCreateCohort = true)
        {
            return new ProviderPermission
            {
                UkPrn = ukPrn,
                CanCreateCohort = canCreateCohort,
                Account = setAccount ? new Account
                {
                    IsLevy = isLevy
                } : null,
                AccountLegalEntity = setAccountLegalEntity ? new AccountLegalEntity
                {
                    AccountLegalEntityName = "test1"
                } : null
            };
        }
    }
}