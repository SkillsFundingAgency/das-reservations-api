using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.GetAccountReservationStatus;

namespace SFA.DAS.Reservations.Application.UnitTests.AccountLegalEntities.Queries.AccountReservationStatus
{
    [TestFixture]
    public class WhenValidatingAGetAccountReservationStatusQuery
    {
        [Test, AutoData]
        public async Task And_No_Account_Id_Then_Invalid(
            GetAccountReservationStatusQuery query,
            GetAccountReservationStatusQueryValidator validator)
        {
            query.AccountId = default(long);

            var result = await validator.ValidateAsync(query);

            result.IsValid().Should().BeFalse();
            result.ValidationDictionary.Keys.Should().Contain(nameof(query.AccountId));
        }

        [Test, AutoData]
        public async Task And_Account_Id_Greater_Than_Zero_Then_Valid(
            GetAccountReservationStatusQuery query,
            GetAccountReservationStatusQueryValidator validator)
        {
            var result = await validator.ValidateAsync(query);

            result.IsValid().Should().BeTrue();
        }
    }
}