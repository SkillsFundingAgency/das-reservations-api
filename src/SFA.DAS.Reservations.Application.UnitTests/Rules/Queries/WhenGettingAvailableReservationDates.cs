using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    [TestFixture]
    public class WhenGettingAvailableReservationDates
    {
        [Test, MoqAutoData]
        public async Task Then_Gets_Account_Id(
            GetAvailableDatesQuery query,
            List<AvailableDateStartWindow> availableDateStartWindows,
            [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
            [Frozen] Mock<IAvailableDatesService> mockDatesService,
            GetAvailableDatesQueryHandler handler)
        {
            await handler.Handle(query, CancellationToken.None);

            mockAleService.Verify(service => service.GetAccountLegalEntity(query.AccountLegalEntityId), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Available_Dates_Service_Is_Called_And_Dates_Returned(
            GetAvailableDatesQuery query,
            AccountLegalEntity accountLegalEntity,
            List<AvailableDateStartWindow> availableDateStartWindows,
            [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
            [Frozen] Mock<IAvailableDatesService> mockDatesService,
            GetAvailableDatesQueryHandler handler)
        {
            //Arrange
            mockAleService
                .Setup(service => service.GetAccountLegalEntity(query.AccountLegalEntityId))
                .ReturnsAsync(accountLegalEntity);
            mockDatesService
                .Setup(x => x.GetAvailableDates(accountLegalEntity.AccountId))
                .Returns(availableDateStartWindows);

            //Act
            var actual = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsAssignableFrom<GetAvailableDatesResult>(actual);
            Assert.AreSame(availableDateStartWindows, actual.AvailableDates);
        }
    }
}