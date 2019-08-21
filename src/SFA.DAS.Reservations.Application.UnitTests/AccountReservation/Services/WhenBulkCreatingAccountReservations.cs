using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Services;
using SFA.DAS.Reservations.Domain.Reservations;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Services
{
    public class WhenBulkCreatingAccountReservations
    {

        [Test, MoqAutoData]
        public async Task Then_The_Number_Of_Reservations_Is_Created_Based_On_The_Param_Passed_In_As_Levy_Reservations(
            [Frozen]Mock<IReservationRepository> repository,
            uint numberOfReservations,
            long accountLegalEntityId,
            long accountId,
            string accountLegalEntityName,
            AccountReservationService service
            )
        {
            
            //Act
            await service.BulkCreateAccountReservation(numberOfReservations, accountLegalEntityId, accountId, accountLegalEntityName, null);

            //Assert
            repository.Verify(x => x.CreateAccountReservations(
                It.Is<List<Domain.Entities.Reservation>>(c=>c.Count.Equals((int)numberOfReservations) 
                    && c.All(y=>y.IsLevyAccount)
                    && c.All(y => y.Status.Equals(0)) 
                    && c.All(y => y.AccountLegalEntityId.Equals(accountLegalEntityId)) 
                    && c.All(y => y.AccountLegalEntityName.Equals(accountLegalEntityName)) 
                    && c.All(y => y.AccountId.Equals(accountId)))), 
                Times.Once);
            
        }


        [Test, MoqAutoData]
        public async Task Then_The_TransferSenderAccountId_Is_Added_To_The_Reservation_If_Provided(
            [Frozen]Mock<IReservationRepository> repository,
            uint numberOfReservations,
            long accountLegalEntityId,
            long accountId,
            long? transferSenderAccountId,
            string accountLegalEntityName,
            AccountReservationService service
        )
        {

            //Act
            await service.BulkCreateAccountReservation(numberOfReservations, accountLegalEntityId, accountId, accountLegalEntityName, transferSenderAccountId);

            //Assert
            repository.Verify(x => x.CreateAccountReservations(
                    It.Is<List<Domain.Entities.Reservation>>(c => c.Count.Equals((int)numberOfReservations)
                                                                  && c.All(y => y.IsLevyAccount)
                                                                  && c.All(y => y.Status.Equals(0))
                                                                  && c.All(y => y.AccountLegalEntityId.Equals(accountLegalEntityId))
                                                                  && c.All(y => y.AccountLegalEntityName.Equals(accountLegalEntityName))
                                                                  && c.All(y => y.TransferSenderAccountId.Equals(transferSenderAccountId))
                                                                  && c.All(y => y.AccountId.Equals(accountId)))),
                Times.Once);

        }


        [Test, MoqAutoData]
        public async Task Then_The_List_Of_Created_Reservation_Ids_Are_Returned(
            [Frozen]Mock<IReservationRepository> repository,
            uint numberOfReservations,
            long accountLegalEntityId,
            long accountId,
            string accountLegalEntityName,
            AccountReservationService service
        )
        {
            //Act
            var actual = await service.BulkCreateAccountReservation(numberOfReservations, accountLegalEntityId, accountId, accountLegalEntityName, null);

            //Assert
            Assert.AreEqual(numberOfReservations, actual.Count);
        }
    }
}
