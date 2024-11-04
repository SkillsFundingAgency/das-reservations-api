using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountLegalEntity;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Validation;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Queries.GetAccountLegalEntity
{
    public class WhenGettingAccountLegalEntity
    {
        private const long ExpectedAccountLegalEntityId = 553234;

        private GetAccountLegalEntityQueryHandler _handler;
        private Mock<IValidator< GetAccountLegalEntityQuery>> _validator;
        private CancellationToken _cancellationToken;
        private  GetAccountLegalEntityQuery _query;
        private Mock<IAccountLegalEntitiesService> _service;
       

        [SetUp]
        public void Arrange()
        {
            _query = new  GetAccountLegalEntityQuery { Id = ExpectedAccountLegalEntityId };
            _validator = new Mock<IValidator< GetAccountLegalEntityQuery>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny< GetAccountLegalEntityQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });
            _cancellationToken = new CancellationToken();
            _service = new Mock<IAccountLegalEntitiesService>();

            _handler = new  GetAccountLegalEntityQueryHandler(_service.Object, _validator.Object);
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
            var errorKey = "Test Error";
            var errorValue = "There has ben an error";
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntityQuery>()))
                .ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { errorKey, errorValue } } });

            //Act + Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(_query, _cancellationToken));
        }

        [Test]
        public async Task Then_The_Return_Type_Is_Assigned_To_The_Response()
        {
            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            actual.Should().BeAssignableTo<GetAccountLegalEntityResult>();
        }

        [Test]
        public async Task Then_The_Repository_Is_Called_With_The_Request_Details()
        {
            //Act
            await _handler.Handle(_query, _cancellationToken);

            //Assert
            _service.Verify(x => x.GetAccountLegalEntity(ExpectedAccountLegalEntityId));
        }

        [Test]
        public async Task Then_The_Values_Are_Returned_In_The_Response()
        {
            //Arrange
            const int legalEntityId = 123;
            const int accountId = 6543;
            const bool agreementSigned = true;
            const bool isLevy = false;
            const string legalEntityName = "Test Entity";
            var accountLegalEntity = new AccountLegalEntity(Guid.Empty, accountId, legalEntityName, legalEntityId, ExpectedAccountLegalEntityId, agreementSigned, isLevy);
            
            _service.Setup(x => x.GetAccountLegalEntity(It.IsAny<long>())).ReturnsAsync(accountLegalEntity);

            //Act
            var actual = await _handler.Handle(_query, _cancellationToken);

            //Assert
            actual.LegalEntity.Should().NotBeNull();
            actual.LegalEntity.AccountId.Should().Be(accountId);
            actual.LegalEntity.AccountLegalEntityName.Should().Be(legalEntityName);
            actual.LegalEntity.LegalEntityId.Should().Be(legalEntityId);
            actual.LegalEntity.AccountLegalEntityId.Should().Be(ExpectedAccountLegalEntityId);
            actual.LegalEntity.AgreementSigned.Should().Be(agreementSigned);
            actual.LegalEntity.IsLevy.Should().Be(isLevy);

        }
    }
}
