using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries.GetAvailableDates;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries;

[TestFixture]
public class WhenValidatingAGetAvailableDatesQuery
{
    [Test, AutoData]
    public async Task And_AccountLegalEntityId_Is_Less_Than_1_Then_Is_Invalid(
        GetAvailableDatesQuery query,
        GetAvailableDatesValidator validator)
    {
        if (query.AccountLegalEntityId > 0) 
            query.AccountLegalEntityId = -query.AccountLegalEntityId;
            
        var result = await validator.ValidateAsync(query);

        result.IsValid().Should().BeFalse();
        result.ValidationDictionary.Keys.Should().Contain(nameof(query.AccountLegalEntityId));
    }

    [Test, AutoData]
    public async Task And_AccountLegalEntityId_Is_Greater_Than_0_Then_Is_Valid(
        GetAvailableDatesQuery query,
        GetAvailableDatesValidator validator)
    {
        var result = await validator.ValidateAsync(query);

        result.IsValid().Should().BeTrue();
    }
}