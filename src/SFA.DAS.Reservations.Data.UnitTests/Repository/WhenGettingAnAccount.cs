using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.Repository;
using SFA.DAS.Reservations.Data.UnitTests.DatabaseMock;
using SFA.DAS.Reservations.Domain.Entities;
using SFA.DAS.Reservations.Domain.Exceptions;

namespace SFA.DAS.Reservations.Data.UnitTests.Repository
{
    public class WhenGettingAnAccount
    {
        private const long ExpectedAccountId = 2;
        private Mock<IReservationsDataContext> _reservationsDataContext;
        private List<Account> _accounts;
        private AccountRepository _accountsRepository;

        [SetUp]
        public void Arrange()
        {
            _accounts = new List<Account>
            {
                new Account
                {
                    Id = 1,
                    Name = "Account One",
                    IsLevy = false
                },
                new Account
                {
                    Id = 2,
                    Name = "Account Two",
                    IsLevy = false
                },
                new Account
                {
                    Id = 3,
                    Name = "Account Three",
                    IsLevy = true
                }
            };
            
            _reservationsDataContext = new Mock<IReservationsDataContext>();
            _reservationsDataContext.Setup(x => x.Accounts).ReturnsDbSet(_accounts);
            _reservationsDataContext.Setup(x => x.Accounts.FindAsync(ExpectedAccountId))
                .ReturnsAsync(_accounts.SingleOrDefault(c => c.Id.Equals(ExpectedAccountId)));

            _accountsRepository = new AccountRepository(_reservationsDataContext.Object);
        }

        [Test]
        public async Task Then_The_Account_Is_Returned_If_It_Exists()
        {
            //Act
            var account = await _accountsRepository.Get(ExpectedAccountId);
            
            //Assert
            account.Should().NotBeNull();
        }

        [Test]
        public void Then_An_Entity_Not_Found_Exception_Is_Thrown()
        {
            //Arrange
            _reservationsDataContext.Setup(x => x.Accounts
                .FindAsync(It.IsAny<long>()))
                .ThrowsAsync(new InvalidOperationException(""));
            
            //Act Assert
            Assert.ThrowsAsync<EntityNotFoundException<Account>>(() => _accountsRepository.Get(5));
        }
    }
}