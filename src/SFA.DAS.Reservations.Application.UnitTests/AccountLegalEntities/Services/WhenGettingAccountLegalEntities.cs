using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Services;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Configuration;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Services
{
    public class WhenGettingAccountLegalEntities
    {
        private AccountLegalEntitiesService _service;
        private Mock<IAccountLegalEntitiesRepository> _repository;
        private List<Domain.Entities.AccountLegalEntity> _legalEntities;
        private Mock<IOptions<ReservationsConfiguration>> _options;

        private const long ExpectedAccountId = 5465478;
        private const int ExpectedReservationLimit = 50;

        [SetUp]
        public void Arrange()
        {
            var account = new Domain.Entities.Account
            {
                Id = 1,
                Name = "Test",
                IsLevy = true,
                ReservationLimit = 3
            };
            
            _legalEntities = new List<Domain.Entities.AccountLegalEntity>
            {
                new Domain.Entities.AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityId = 2,
                    AccountLegalEntityName = "Test",
                    LegalEntityId = 43,
                    AgreementSigned = false,
                    ProviderPermissions = null,
                    Account = account
                },
                new Domain.Entities.AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityId = 3,
                    AccountLegalEntityName = "Test 2",
                    LegalEntityId = 54,
                    AgreementSigned = true,
                    ProviderPermissions = null,
                    Account = account
                }
            };
            
            _repository = new Mock<IAccountLegalEntitiesRepository>();
            _repository.Setup(x => x.GetByAccountId(ExpectedAccountId)).ReturnsAsync(_legalEntities);

            _options = new Mock<IOptions<ReservationsConfiguration>>();
            _options.Setup(x => x.Value.MaxNumberOfReservations).Returns(ExpectedReservationLimit);

            _service = new AccountLegalEntitiesService(_repository.Object, _options.Object);
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_And_Entities_Returned()
        {
            //Act
            var actual = await _service.GetAccountLegalEntities(ExpectedAccountId);

            //Assert
            _repository.Verify(x=>x.GetByAccountId(ExpectedAccountId), Times.Once);
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public async Task Then_The_Return_Model_Is_Mapped_To_The_Domain_Model()
        {
            //Act
            var actual = await _service.GetAccountLegalEntities(ExpectedAccountId);

            //Assert
            Assert.IsAssignableFrom<List<AccountLegalEntity>>(actual);
            Assert.AreEqual(2, actual.Count);
            actual.Should().BeEquivalentTo(_legalEntities, options =>
                options.Excluding(p => p.ProviderPermissions).Excluding(p => p.Account));
        }
    }
}
