using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Account.Services;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Configuration;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Services
{
    public class WhenGettingAccount
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_Gets_Entity_From_Repository(
            Domain.Entities.Account account,
            [Frozen] Mock<IAccountRepository> mockRepository,
            AccountsService service)
        {
            mockRepository
                .Setup(repository => repository.Get(account.Id))
                .ReturnsAsync(account);

            await service.GetAccount(account.Id);

            mockRepository.Verify(repository => repository.Get(account.Id), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_Maps_Entity_To_Domain_Model(
            Domain.Entities.Account account,
            [Frozen] Mock<IAccountRepository> mockRepository,
            AccountsService service)
        {

            mockRepository
                .Setup(repository => repository.Get(account.Id))
                .ReturnsAsync(account);
            
            var actual = await service.GetAccount(account.Id);

            actual.Should().BeOfType<Domain.Account.Account>();
            account.Should().BeEquivalentTo(actual);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_If_There_Is_No_Reservation_Limit_On_The_Account_It_Is_Taken_From_Config(
            Domain.Entities.Account account,
            int numberOfReservations,
            [Frozen] Mock<IOptions<ReservationsConfiguration>> configuration,
            [Frozen] Mock<IAccountRepository> mockRepository
            )
        {
            configuration.Setup(x => x.Value.MaxNumberOfReservations).Returns(numberOfReservations);
            account.ReservationLimit = null;
            mockRepository
                .Setup(repository => repository.Get(account.Id))
                .ReturnsAsync(account);
            
            var service = new AccountsService(mockRepository.Object, configuration.Object);
            
            var actual = await service.GetAccount(account.Id);

            actual.ReservationLimit.Should().Be(numberOfReservations);
        }
    }
}