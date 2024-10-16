using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntities;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Queries.GetAccountLegalEntities
{
    public class WhenGettingAccountLegalEntities
    {
        private GetAccountLegalEntitiesQueryHandler _handler;
        private Mock<IValidator<GetAccountLegalEntitiesQuery>> _validator;
        private CancellationToken _cancellationToken;
        private GetAccountLegalEntitiesQuery _query;
        private Mock<IAccountLegalEntitiesService> _service;
        private const long ExpectedAccountId = 553234;
        private const string ExpectedAccountLegalEntityName = "TestName";

        [SetUp]
        public void Arrange()
        {
            _query = new GetAccountLegalEntitiesQuery { AccountId = ExpectedAccountId };
            _validator = new Mock<IValidator<GetAccountLegalEntitiesQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntitiesQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _cancellationToken = new CancellationToken();
            _service = new Mock<IAccountLegalEntitiesService>();

            _handler = new GetAccountLegalEntitiesQueryHandler(_validator.Object, _service.Object);
        }

        [Test]
        public async Task Then_The_Query_Is_Validated()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _validator.Verify(x => x.ValidateAsync(_query), Times.Once);

        }

        [Test]
        public void Then_If_The_Query_Fails_Validation_Then_An_Argument_Exception_Is_Thrown()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntitiesQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

            //Act Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(_query, _cancellationToken));
        }

        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            actual.Should().BeAssignableTo<GetAccountLegalEntitiesResponse>();
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_With_The_Request_Details()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _service.Verify(x => x.GetAccountLegalEntities(ExpectedAccountId));
        }

        [Test]
        public async Task Then_The_Values_Are_Returned_In_The_Response()
        {
            //Arrange
            var legalEntityId = 123;
            var accountLegalEntityId = 6543;            
            var agreementSigned = true;
            var isLevy = false;
            var accountLegalEntity = new AccountLegalEntity(Guid.Empty, ExpectedAccountId, ExpectedAccountLegalEntityName, legalEntityId, accountLegalEntityId, agreementSigned, isLevy);
            _service.Setup(x => x.GetAccountLegalEntities(ExpectedAccountId)).ReturnsAsync(new List<AccountLegalEntity> { accountLegalEntity });

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            actual.AccountLegalEntities.Should().NotBeNull();
            var firstEntity = actual.AccountLegalEntities[0];

            firstEntity.AccountId.Should().Be(ExpectedAccountId);
            firstEntity.AccountLegalEntityName.Should().Be(ExpectedAccountLegalEntityName);
            firstEntity.LegalEntityId.Should().Be(legalEntityId);
            firstEntity.AccountLegalEntityId.Should().Be(accountLegalEntityId);
            firstEntity.AgreementSigned.Should().Be(agreementSigned);
            firstEntity.IsLevy.Should().Be(isLevy);
        }
    }
}
