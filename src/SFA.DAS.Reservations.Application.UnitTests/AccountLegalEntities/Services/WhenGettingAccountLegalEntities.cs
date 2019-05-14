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
            _legalEntities = new List<Domain.Entities.AccountLegalEntity>
            {
                new Domain.Entities.AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityId = 2,
                    AccountLegalEntityName = "Test",
                    LegalEntityId = 43,
                    ReservationLimit = 4
                },
                new Domain.Entities.AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityId = 3,
                    AccountLegalEntityName = "Test 2",
                    LegalEntityId = 54,
                    ReservationLimit = 4
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
            Assert.AreEqual(2,actual.Count);
            actual.Should().BeEquivalentTo(_legalEntities);
        }

        [Test]
        public async Task Then_If_No_Limit_Has_Been_Set_On_The_Number_Of_Reservations_Then_The_Default_Limit_Is_Used()
        {
            //Arrange
            _repository.Setup(x => x.GetByAccountId(ExpectedAccountId)).ReturnsAsync(new List<Domain.Entities.AccountLegalEntity>
            {
                new Domain.Entities.AccountLegalEntity
                {
                    Id = Guid.NewGuid(),
                    AccountId = 1,
                    AccountLegalEntityId = 1,
                    AccountLegalEntityName = "Test",
                    LegalEntityId = 4,
                    ReservationLimit = null
                }
            });

            //Act
            var actual = await _service.GetAccountLegalEntities(ExpectedAccountId);

            //Assert
            Assert.AreEqual(ExpectedReservationLimit,actual.First().ReservationLimit);
        }
    }
}
