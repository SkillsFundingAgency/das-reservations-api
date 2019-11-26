using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Domain.Entities;

namespace SFA.DAS.Reservations.Domain.UnitTests.ProviderPermissions
{
    public class WhenCreatingAProviderPermissionFromEntity
    {
        [Test, AutoData]
        public void Then_The_Entity_Is_Mapped_To_The_Domain_Object(
            long accountId,
            long accountLegalEntityId,
            uint ukPrn,
            string accountLegalEntityName,
            AgreementType agreementType
            )
        {
            //Arrange
            var entity = new ProviderPermission
            {
                AccountId = accountId,
                UkPrn = ukPrn,
                AccountLegalEntityId = accountLegalEntityId,
                AccountLegalEntity = new AccountLegalEntity
                {
                    AgreementSigned = true,
                    AccountLegalEntityName = accountLegalEntityName,
                    AccountId = accountId,
                    AccountLegalEntityId = accountLegalEntityId,
                    AgreementType = agreementType
                }
            };
            
            //Act
            var actual = new Domain.ProviderPermissions.ProviderPermission(entity);
            
            //Assert
            actual.AccountId.Should().Be(accountId);
            actual.UkPrn.Should().Be(ukPrn);
            actual.AccountLegalEntityId.Should().Be(accountLegalEntityId);
            actual.AgreementSigned.Should().BeTrue();
            actual.AccountLegalEntityName.Should().Be(accountLegalEntityName);
            actual.AgreementType.Should().Be(agreementType);
        }
    }
}