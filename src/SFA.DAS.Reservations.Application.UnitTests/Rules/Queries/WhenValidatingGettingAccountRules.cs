﻿using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Application.Rules.Queries;

namespace SFA.DAS.Reservations.Application.UnitTests.Rules.Queries
{
    public class WhenValidatingGettingAccountRules
    {
        private GetAccountRulesValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountRulesValidator();
        }

        [Test]
        public async Task Then_The_Query_Is_Invalid_If_The_Required_Fields_Are_Not_Passed_And_Validation_Errors_Returned()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountRulesQuery());

            //Assert
            actual.IsValid().Should().BeFalse();
            actual.ValidationDictionary.Should().ContainValue("AccountId has not been supplied");
        }

        [Test]
        public async Task Then_The_Query_Is_Valid_If_The_Values_Are_Valid()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountRulesQuery { AccountId = 99432 });

            //Assert
            actual.IsValid().Should().BeTrue();
        }
    }
}
