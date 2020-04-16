using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Domain.UnitTests.Account
{
    public class WhenMappingAnAccountEntityToDomain
    {
        [Test,RecursiveMoqAutoData]
        public void Then_The_Entity_Is_Mapped_To_The_Domain(
            Domain.Entities.Account accountEntity)
        {
            //Act
            var actual = new Domain.Account.Account(accountEntity,1);
            
            //Assert
            actual.Should()
                .BeEquivalentTo(accountEntity, 
                    options=>options
                        .Excluding(c=>c.ProviderPermissions)
                        .Excluding(c=>c.AccountLegalEntities));
        }

        [Test, RecursiveMoqAutoData]
        public void Then_If_There_Is_No_Limit_On_The_Account_The_Global_Value_Is_Used(
            Domain.Entities.Account accountEntity, int globalReservationLimit)
        {
            //Arrange
            accountEntity.ReservationLimit = null;
            
            //Act
            var actual = new Domain.Account.Account(accountEntity, globalReservationLimit);
            
            //Assert
            Assert.AreEqual(globalReservationLimit,actual.ReservationLimit);
        }
    }
}