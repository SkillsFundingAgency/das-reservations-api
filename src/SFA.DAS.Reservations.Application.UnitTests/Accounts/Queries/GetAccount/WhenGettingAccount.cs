using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Account.Queries.GetAccount;
using SFA.DAS.Reservations.Domain.Account;
using SFA.DAS.Reservations.Domain.Exceptions;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Accounts.Queries.GetAccount
{
    public class WhenGettingAccount
    {
        [Test, RecursiveMoqAutoData]
        public void And_Fails_Validation_Then_Throws_ValidationException(
            GetAccountQuery query,
            string propertyName,
            Domain.Account.Account account,
            [Frozen] Mock<IValidator<GetAccountQuery>> mockValidator,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountsService> mockService,
            GetAccountQueryHandler handler)
        {
            // Arrange
            validationResult.AddError(propertyName);
            mockValidator
                .Setup(validator => validator.ValidateAsync(It.IsAny<GetAccountQuery>()))
                .ReturnsAsync(validationResult);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<ArgumentException>()
                .WithMessage($"*{propertyName}*");
        }

        [Test, MoqAutoData]
        public void And_No_Account_Then_Throws_EntityNotFoundException(
            GetAccountQuery query,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountsService> mockService,
            GetAccountQueryHandler handler)
        {
            // Arrange
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetAccount(It.IsAny<long>()))
                .ReturnsAsync((Domain.Account.Account)null);

            // Act
            var act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            act.Should().ThrowAsync<EntityNotFoundException<Domain.Entities.Account>>();
        }
        
        [Test, RecursiveMoqAutoData]
        public async Task Then_Gets_Account_Details_From_Service(
            GetAccountQuery query,
            Domain.Account.Account account,
            [Frozen] ValidationResult validationResult,
            [Frozen] Mock<IAccountsService> mockService,
            GetAccountQueryHandler handler)
        {
            validationResult.ValidationDictionary.Clear();
            mockService
                .Setup(service => service.GetAccount(query.Id))
                .ReturnsAsync(account);

            var result = await handler.Handle(query, CancellationToken.None);

            result.Account.Should().BeEquivalentTo(account);
        }
    }
}