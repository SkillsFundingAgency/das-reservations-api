using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Services
{
    [TestFixture]
    public class WhenGettingAccountLegalEntity
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Entity_From_Repository(
            long accountLegalEntityId,
            [Frozen] Mock<IAccountLegalEntitiesRepository> mockRepository,
            AccountLegalEntitiesService service)
        {
            var accountLegalEntityEntity = BuildAccountLegalEntity(accountLegalEntityId);
            mockRepository
                .Setup(repository => repository.Get(accountLegalEntityId))
                .ReturnsAsync(accountLegalEntityEntity);

            await service.GetAccountLegalEntity(accountLegalEntityId);

            mockRepository.Verify(repository => repository.Get(accountLegalEntityId), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_Maps_Entity_To_Domain_Model(
            long accountLegalEntityId,
            [Frozen] Mock<IAccountLegalEntitiesRepository> mockRepository,
            AccountLegalEntitiesService service)
        {
            
            var accountLegalEntityEntity = BuildAccountLegalEntity(accountLegalEntityId);
            mockRepository
                .Setup(repository => repository.Get(accountLegalEntityId))
                .ReturnsAsync(accountLegalEntityEntity);

            var accountLegalEntity = await service.GetAccountLegalEntity(accountLegalEntityId);

            accountLegalEntity.Should().BeOfType<AccountLegalEntity>();
            accountLegalEntity.Should().BeEquivalentTo(accountLegalEntityEntity, options=>options.Excluding(p=>p.ProviderPermissions));
        }

        private static Domain.Entities.AccountLegalEntity BuildAccountLegalEntity(long accountLegalEntityId)
        {
            return new Domain.Entities.AccountLegalEntity
            {
                AccountId = 123,
                Id = Guid.NewGuid(),
                AgreementSigned = true,
                AgreementType = AgreementType.NonLevyExpressionOfInterest,
                IsLevy = false,
                ProviderPermissions = null,
                ReservationLimit = 10,
                LegalEntityId = 23,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntityName = "Test Name"
            };
        }
    }
}