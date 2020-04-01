using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Account.Services;
using SFA.DAS.Reservations.Domain.Account;
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
    }
}