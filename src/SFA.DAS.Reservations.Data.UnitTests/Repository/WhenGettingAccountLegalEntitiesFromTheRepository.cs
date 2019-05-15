using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;

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
        public async Task Then_The_LegalEntities_That_Have_An_Agreement_Signed_Are_Returned_By_AccountId()
        {
            //Act
            var actual = await _legalEntityRepository.GetByAccountIdWithAgreementSigned(1);

            //Assert
            Assert.IsAssignableFrom<List<AccountLegalEntity>>(actual);
            Assert.IsNotEmpty(actual);
            Assert.AreEqual(2, actual.Count);
        }

        [Test]
        public async Task Then_If_There_Are_No_Results_An_Empty_List_Is_Returned()
        {
            //Act
            var actual = await _legalEntityRepository.GetByAccountIdWithAgreementSigned(3);

            //Assert
            Assert.IsEmpty(actual);
        }
    }
}
