using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.ProviderPermissions.Services;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermissions.Services
{
    public class WhenGettingProviderPermissions
    {
        [Test, MoqAutoData]
        public async Task Then_The_ProviderPermissions_Are_Returned_From_The_Repository_By_ProviderId(
            uint providerId,
            [Frozen] Mock<IProviderPermissionRepository> providerPermissionRepository,
            ProviderPermissionsService service)
        {
            //Arrange
            var providerPermissions = new List<Domain.Entities.ProviderPermission>
            {
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = new AccountLegalEntity(),
                    AccountLegalEntityId = 2,
                    CanCreateCohort = true,
                    Account = new Domain.Entities.Account{Name = "test"}
                }
            };
            providerPermissionRepository.Setup(x => x.GetByProviderId(providerId)).ReturnsAsync(providerPermissions);

            //Act
            await service.GetPermissionsByProviderId(providerId);

            //Assert
            providerPermissionRepository.Verify(x=>x.GetByProviderId(providerId), Times.Once);

        }

        [Test, MoqAutoData]
        public async Task Then_Only_Non_Levy_Legal_Entities_With_Permissions_Are_Returned_And_Mapped_To_The_Domain_Object(
            uint providerId,
            string accountLegalEntityName,
            [Frozen] Mock<IProviderPermissionRepository> providerPermissionRepository,
            ProviderPermissionsService service)
        {
            //Arrange
            var providerPermissions = new List<Domain.Entities.ProviderPermission>
            {
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = new AccountLegalEntity
                    {
                        AccountLegalEntityName = accountLegalEntityName
                    },
                    AccountLegalEntityId = 2,
                    CanCreateCohort = true,
                    Account = new Domain.Entities.Account{Name = "test", IsLevy = false}
                },
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = new AccountLegalEntity
                    {
                        AccountLegalEntityName = accountLegalEntityName
                    },
                    AccountLegalEntityId = 2,
                    CanCreateCohort = true,
                    Account = new Domain.Entities.Account{Name = "test", IsLevy = false}
                },
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = new AccountLegalEntity
                    {
                        AccountLegalEntityName = accountLegalEntityName
                    },
                    AccountLegalEntityId = 2,
                    CanCreateCohort = true,
                    Account = new Domain.Entities.Account{Name = "test", IsLevy = true}
                },
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = null,
                    AccountLegalEntityId = 2,
                    CanCreateCohort = true,
                    Account = null
                },
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = new AccountLegalEntity
                    {
                        AccountLegalEntityName = accountLegalEntityName
                    },
                    AccountLegalEntityId = 2,
                    CanCreateCohort = true,
                    Account = null
                },
                new Domain.Entities.ProviderPermission
                {
                    AccountId = 1,
                    UkPrn = providerId,
                    AccountLegalEntity = new AccountLegalEntity
                    {
                        AccountLegalEntityName = accountLegalEntityName
                    },
                    AccountLegalEntityId = 2,
                    CanCreateCohort = false,
                    Account = new Domain.Entities.Account{Name = "test", IsLevy = false}
                }
            };
            providerPermissionRepository.Setup(x => x.GetByProviderId(providerId)).ReturnsAsync(providerPermissions);
            
            //Act
            var actual = await service.GetPermissionsByProviderId(providerId);
            
            //Assert
            Assert.AreEqual(2, actual.Count);
            
        }
    }
}