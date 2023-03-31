using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Reservations.Application.Rules.Queries;
using SFA.DAS.Reservations.Domain.AccountLegalEntities;
using SFA.DAS.Reservations.Domain.Rules;
using SFA.DAS.Reservations.Domain.Validation;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    [TestFixture]
    public class WhenGettingAvailableReservationDates
    {
        [Test, MoqAutoData]
        public void And_Query_Invalid_Then_Throws_InvalidArgumentException(
            GetAvailableDatesQuery query,
            ValidationResult validationResult,
            string propertyName,
            List<AvailableDateStartWindow> availableDateStartWindows,
            [Frozen] Mock<IValidator<GetAvailableDatesQuery>> mockValidator,
            [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
            [Frozen] Mock<IAvailableDatesService> mockDatesService,
            GetAvailableDatesQueryHandler handler)
        {
            validationResult.AddError(propertyName);
            mockValidator
                .Setup(validator => validator.ValidateAsync(query))
                .ReturnsAsync(validationResult);

            var act = new Func<Task>(async ()  => await handler.Handle(query, CancellationToken.None));

            act.Should().Throw<ArgumentException>()
                .WithMessage("The following parameters have failed validation*")
                .Which.ParamName.Should().Contain(propertyName);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Available_Dates_Service_Is_Called_And_Dates_Returned(
            GetAvailableDatesQuery query,
            [Frozen] ValidationResult validationResult,
            AccountLegalEntity accountLegalEntity,
            List<AvailableDateStartWindow> availableDateStartWindows,
            [Frozen] Mock<IAccountLegalEntitiesService> mockAleService,
            [Frozen] Mock<IAvailableDatesService> mockDatesService,
            GetAvailableDatesQueryHandler handler)
        {
            //Arrange
            validationResult.ValidationDictionary.Clear();
            mockAleService
                .Setup(service => service.GetAccountLegalEntity(query.AccountLegalEntityId))
                .ReturnsAsync(accountLegalEntity);
            mockDatesService
                .Setup(x => x.GetAvailableDates())
                .Returns(availableDateStartWindows);

            //Act
            var actual = await handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.IsAssignableFrom<GetAvailableDatesResult>(actual);
            Assert.AreSame(availableDateStartWindows, actual.AvailableDates);
        }
    }
}