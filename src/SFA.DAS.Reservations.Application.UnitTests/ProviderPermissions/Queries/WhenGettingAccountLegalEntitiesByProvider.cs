using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.ProviderPermissions.Queries;
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
            List<ProviderPermission> permissions,
            [Frozen] Mock<IProviderPermissionRepository> repository,
            [Frozen] Mock<IValidator<GetAccountLegalEntitiesForProviderQuery>> validator,
            GetAccountLegalEntitiesForProviderQueryHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(query)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string>()
            });

            repository.Setup(x => x.GetAllowedNonLevyPermissionsForProvider(query.ProviderId)).ReturnsAsync(permissions);

            //Act
            await handler.Handle(query, CancellationToken.None);

            //Act
            validator.Verify(x => x.ValidateAsync(query), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Service_Is_Called_With_The_Supplied_ProviderId_And_Results_Returned_In_The_Response(
            GetAccountLegalEntitiesForProviderQuery query,
            List<ProviderPermission> permissions,
            [Frozen] Mock<IProviderPermissionRepository> repository,
            [Frozen] Mock<IValidator<GetAccountLegalEntitiesForProviderQuery>> validator,
            GetAccountLegalEntitiesForProviderQueryHandler handler)
        {
            //Arrange
            validator.Setup(x => x.ValidateAsync(query)).ReturnsAsync(new ValidationResult
            {
                ValidationDictionary = new Dictionary<string, string>()
            });
            
            repository.Setup(x => x.GetAllowedNonLevyPermissionsForProvider(query.ProviderId)).ReturnsAsync(permissions);

            //Act
            var actual = await handler.Handle(query, CancellationToken.None);

            //Assert
            repository.Verify(x => x.GetAllowedNonLevyPermissionsForProvider(query.ProviderId), Times.Once);
            actual.ProviderPermissions.Should().NotBeNull();
        }

        [Test, MoqAutoData]
        public void Then_An_Argument_Excpetion_Is_Thrown_If_The_Query_Is_Not_Valid(
            GetAccountLegalEntitiesForProviderQuery query,
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
    }
}