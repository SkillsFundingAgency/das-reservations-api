using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    [TestFixture]
    public class WhenGettingAccountLegalEntity
    {
        [Test, MoqAutoData]
        public void And_ALE_Not_Found_Then_Throws_Exception(
            long accountLegalEntityId,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] Mock<IReservationsDataContext> mockDataContext,
            AccountLegalEntityRepository repository)
        {
            accountLegalEntityId = EnsureIdIsUnique(accountLegalEntities, accountLegalEntityId);
            mockDataContext
                .Setup(context => context.AccountLegalEntities)
                .ReturnsDbSet(accountLegalEntities);

            var act = new Func<Task>(async () => await repository.Get(accountLegalEntityId));

            act.Should().Throw<EntityNotFoundException<AccountLegalEntity>>()
                .WithInnerException<InvalidOperationException>();
        }

        [Test, MoqAutoData]
        public async Task Then_Returns_ALE_From_Context(
            long accountLegalEntityId,
            List<AccountLegalEntity> accountLegalEntities,
            [Frozen] Mock<IReservationsDataContext> mockDataContext,
            AccountLegalEntityRepository repository)
        {
            accountLegalEntities[1].AccountLegalEntityId = accountLegalEntityId;
            mockDataContext
                .Setup(context => context.AccountLegalEntities)
                .ReturnsDbSet(accountLegalEntities);

            var ale = await repository.Get(accountLegalEntityId);

            ale.Should().BeSameAs(accountLegalEntities[1]);
        }

        private static long EnsureIdIsUnique(IEnumerable<AccountLegalEntity> accountLegalEntities, long accountLegalEntityId)
        {
            accountLegalEntities
                .OrderBy(entity => entity.AccountLegalEntityId).ToList()
                .ForEach(entity =>
                {
                    if (entity.AccountLegalEntityId == accountLegalEntityId)
                        accountLegalEntityId = entity.AccountLegalEntityId + 1;
                });
            return accountLegalEntityId;
        }
    }
}