using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.BulkCreateAccountReservations;
using SFA.DAS.Reservations.Data;
using TechTalk.SpecFlow;
namespace SFA.DAS.Reservations.Api.AcceptanceTests.Steps
{
    [Binding]
    public class BulkCreateSteps : StepsBase
    {
        private int _reservationCount;
        private long? _transferSenderId = null;

        public BulkCreateSteps(TestData testData, TestResults testResults, TestServiceProvider serviceProvider) : base(testData, testResults, serviceProvider)
        {
        }

        [When(@"I bulk create (.*) reservations")]
        public void WhenIBulkCreateReservations(uint reservationCount)
        {
            var controller = Services.GetService<ReservationsController>();

            var bulkCreateResult = (CreatedResult) Task.FromResult(controller.BulkCreate(AccountLegalEntityId, new BulkReservation
            {
                Count = reservationCount,
                TransferSenderId = _transferSenderId
            }).Result).Result;

            _reservationCount = ((BulkCreateAccountReservationsResult) bulkCreateResult.Value).ReservationIds.Count();
        }

        [Given(@"I am receiving transfer funds")]
        public void GivenIAmReceivingTransferFunds()
        {
            _transferSenderId = 12314;
        }


        [Then(@"(.*) levy reservations are created")]
        public void ThenLevyReservationsAreCreated(int expectedReservations)
        {
            expectedReservations.Should().Be(_reservationCount);
        }

        [Then(@"the transfer id is added to the (.*) reservations")]
        public void ThenTheTransferIdIsAddedToTheReservation(int expectedReservations)
        {
            var dataContext = Services.GetService<ReservationsDataContext>();

            var reservations = dataContext.Reservations.Count(c =>
                c.TransferSenderAccountId.Equals(_transferSenderId) &&
                c.AccountLegalEntityId.Equals(AccountLegalEntityId) && c.IsLevyAccount);

            expectedReservations.Should().Be(reservations);
        }

    }
}
