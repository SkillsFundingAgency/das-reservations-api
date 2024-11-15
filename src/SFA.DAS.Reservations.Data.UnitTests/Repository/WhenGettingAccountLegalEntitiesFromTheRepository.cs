using System;
using System.Collections.Generic;
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
    public class WhenGettingAccountLegalEntitiesFromTheRepository
    {
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private AccountLegalEntityRepository _legalEntityRepository;
        private List<AccountLegalEntity> _legalEntities;

        [SetUp]
        public void Arrange()
        {
            _legalEntities = new List<AccountLegalEntity>
            {
                new AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityName = "Test",
                    AgreementSigned = true
                },
                new AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityName = "Test 1",
                    AgreementSigned = true
                },
                new AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityName = "Test 3",
                    AgreementSigned = false
                },
                new AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 2,
                    AccountLegalEntityName = "Test 2",
                    AgreementSigned = true
                }
            };

            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.AccountLegalEntities).ReturnsDbSet(_legalEntities);

            _legalEntityRepository = new AccountLegalEntityRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_The_LegalEntities_Are_Returned_By_AccountId()
        {
            //Act
            var actual = await _legalEntityRepository.GetByAccountId(1);

            //Assert
            actual.Should().BeAssignableTo<List<AccountLegalEntity>>();
            actual.Should().NotBeEmpty();
            actual.Count.Should().Be(3);
        }

        [Test]
        public async Task Then_If_There_Are_No_Results_An_Empty_List_Is_Returned()
        {
            //Act
            var actual = await _legalEntityRepository.GetByAccountId(3);

            //Assert
            actual.Should().BeEmpty();
        }
        
        [Test, MoqAutoData]
        public async Task And_ALE_Not_Found_Then_Throws_Exception(
            long accountLegalEntityId,
            [Frozen] Mock<IReservationsDataContext> mockDataContext,
            AccountLegalEntityRepository repository)
        {
            mockDataContext
                .Setup(context => context.AccountLegalEntities)
                .Throws(new InvalidOperationException()); ;

            var act = async () => await repository.Get(accountLegalEntityId);

            act
                .Should()
                .ThrowAsync<EntityNotFoundException<AccountLegalEntity>>()
                .WithInnerException(typeof(InvalidOperationException));
        }
    }
}
