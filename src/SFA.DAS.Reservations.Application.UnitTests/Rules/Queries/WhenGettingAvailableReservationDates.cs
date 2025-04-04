﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
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
        public async Task And_Query_Invalid_Then_Throws_InvalidArgumentException(
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

            var ex = await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("The following parameters have failed validation*");
                
            ex.Subject.Single().ParamName.Should().Contain(propertyName);
        }

        [Test, MoqAutoData]
        public async Task Then_The_Available_Dates_Service_Is_Called_And_Dates_Returned(
            GetAvailableDatesQuery query,
            [Frozen] ValidationResult validationResult,
            AccountLegalEntity accountLegalEntity,
            AvailableDateStartWindow previousMonth,
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
            actual.AvailableDates.Should().BeEquivalentTo(availableDateStartWindows);
        }
    }
}