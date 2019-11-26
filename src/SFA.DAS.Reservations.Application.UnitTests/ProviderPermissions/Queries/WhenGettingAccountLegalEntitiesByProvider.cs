using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;
using ProviderPermission = SFA.DAS.Reservations.Domain.ProviderPermissions.ProviderPermission;

namespace SFA.DAS.Reservations.Application.UnitTests.ProviderPermissions.Queries
{
    public class WhenGettingAccountLegalEntitiesByProvider
    {
        [Test, MoqAutoData]
        public async Task Then_The_Query_Is_Validated(
            GetAccountLegalEntitiesForProviderQuery query,
            [Frozen] Mock<IProviderPermissionService> service,
            [Frozen] Mock<IValidator<GetAccountLegalEntitiesForProviderQuery>> validator,
            GetAccountLegalEntitiesForProviderQueryHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(query)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string>()
            });
            SetupProviderPermissionService(query, service);

            //Act
            await handler.Handle(query, CancellationToken.None);

            //Act
            validator.Verify(x => x.ValidateAsync(query), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_With_The_Supplied_ProviderId_And_Results_Returned_In_The_Response(
            GetAccountLegalEntitiesForProviderQuery query,
            [Frozen] Mock<IProviderPermissionService> service,
            [Frozen] Mock<IValidator<GetAccountLegalEntitiesForProviderQuery>> validator,
            GetAccountLegalEntitiesForProviderQueryHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(query)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string>()
            });
            ;
            SetupProviderPermissionService(query, service);

            //Act
            var actual = await handler.Handle(query, CancellationToken.None);

            //Assert
            service.Verify(x => x.GetPermissionsByProviderId(query.ProviderId), Times.Once);
            Assert.IsNotNull(actual.ProviderPermissions);
        }

        [Test, MoqAutoData]
        public void Then_An_Argument_Excpetion_Is_Thrown_If_The_Query_Is_Not_Valid(
            GetAccountLegalEntitiesForProviderQuery query,
            [Frozen] Mock<IProviderPermissionService> service,
            [Frozen] Mock<IValidator<GetAccountLegalEntitiesForProviderQuery>> validator,
            GetAccountLegalEntitiesForProviderQueryHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntitiesForProviderQuery>()))
                .ReturnsAsync(new ValidationResult
                    { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await handler.Handle(query, CancellationToken.None));
        }

        private static void SetupProviderPermissionService(GetAccountLegalEntitiesForProviderQuery query, Mock<IProviderPermissionService> service)
        {
            service.Setup(x => x.GetPermissionsByProviderId(query.ProviderId)).ReturnsAsync(
                new List<ProviderPermission>
                {
                    new ProviderPermission(new Domain.Entities.ProviderPermission
                    {
                        AccountId = 1,
                        AccountLegalEntity = new AccountLegalEntity
                        {
                            AccountLegalEntityName = "test"
                        }
                    })
                });
        }
    }
}