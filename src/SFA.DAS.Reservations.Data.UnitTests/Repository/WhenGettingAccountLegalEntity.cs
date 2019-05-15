using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    [TestFixture]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute")]
    public class WhenGettingAccountLegalEntity
    {
        [Test, AutoData]
        public async Task And_ALE_Not_Found_Then_Throws_Exception(
            long accountLegalEntityId,
            List<AccountLegalEntity> accountLegalEntities)
        {
            var mockDataContext = new Mock<IReservationsDataContext>();
            mockDataContext
                .Setup(context => context.AccountLegalEntities)
                .ReturnsDbSet(accountLegalEntities);

            var repository = new AccountLegalEntityRepository(mockDataContext.Object);

            var exception = Assert.ThrowsAsync<EntityNotFoundException>(() => repository.Get(accountLegalEntityId));
            Assert.AreEqual("AccountLegalEntity was not found", exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
        }

        [Test, AutoData]
        public async Task Then_Returns_ALE_From_Context(
            long accountLegalEntityId,
            List<AccountLegalEntity> accountLegalEntities)
        {
            accountLegalEntities[1].AccountLegalEntityId = accountLegalEntityId;
            var mockDataContext = new Mock<IReservationsDataContext>();
            mockDataContext
                .Setup(context => context.AccountLegalEntities)
                .ReturnsDbSet(accountLegalEntities);

            var repository = new AccountLegalEntityRepository(mockDataContext.Object);

            var ale = await repository.Get(accountLegalEntityId);

            Assert.AreSame(accountLegalEntities[1], ale);
        }
    }
}