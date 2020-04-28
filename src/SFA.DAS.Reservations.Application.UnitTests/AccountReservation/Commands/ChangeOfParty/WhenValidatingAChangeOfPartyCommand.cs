using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountReservations.Commands.ChangeOfParty;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountReservation.Commands.ChangeOfParty
{
    public class WhenValidatingAChangeOfPartyCommand
    {
        [Test, AutoData]
        public async Task And_Default_Guid_Then_Not_Valid(
            ChangeOfPartyCommand command,
            ChangeOfPartyCommandValidator validator)
        {
            command.ReservationId = Guid.Empty;
            command.AccountLegalEntityId = null;

            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Keys.Should().Contain(nameof(command.ReservationId));
        }

        [Test, AutoData]
        public async Task And_Guid_Only_Then_Not_Valid(
            ChangeOfPartyCommand command,
            ChangeOfPartyCommandValidator validator)
        {
            command.AccountLegalEntityId = null;
            command.ProviderId = null;

            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Keys.Should().Contain(nameof(command.AccountLegalEntityId));
        }

        [Test, AutoData]
        public async Task And_Both_AccountLegalEntityId_And_ProviderId_Then_Not_Valid(
            ChangeOfPartyCommand command,
            ChangeOfPartyCommandValidator validator)
        {
            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Keys.Should().Contain(nameof(command.ProviderId));
        }

        [Test, AutoData]
        public async Task And_Guid_And_AccountLegalEntityId_Then_Is_Valid(
            ChangeOfPartyCommand command,
            ChangeOfPartyCommandValidator validator)
        {
            command.ProviderId = null;

            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeTrue();
        }

        [Test, AutoData]
        public async Task And_Guid_And_ProviderId_Then_Is_Valid(
            ChangeOfPartyCommand command,
            ChangeOfPartyCommandValidator validator)
        {
            command.AccountLegalEntityId = null;

            var result = await validator.ValidateAsync(command);

            result.IsValid().Should().BeTrue();
        }
    }
}