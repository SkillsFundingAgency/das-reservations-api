﻿using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.UnitTests.Rules
{
    public class WhenCreatingUserRuleAcknowledgement
    {
        [Test]
        public void Then_The_UkPrn_Is_Set_If_The_Id_Is_Numeric()
        {
            //Arrange
            var expectedUkPrn = 45345;

            //Act
            var actual = new UserRuleAcknowledgement(expectedUkPrn.ToString(), 1,RuleType.GlobalRule);

            //Assert
            actual.UkPrn.Should().Be(expectedUkPrn);
        }

        [Test]
        public void Then_The_UserId_Is_Set_If_The_Id_Is_A_Guid()
        {
            //Arrange
            var expectedUserId = Guid.NewGuid();

            //Act
            var actual = new UserRuleAcknowledgement(expectedUserId.ToString(), 1, RuleType.GlobalRule);

            //Assert
            actual.UserId.Should().Be(expectedUserId);
        }

        [TestCase(1)]
        [TestCase(2)]
        public void Then_The_RuleId_Is_Correctly_Set_Based_On_The_Type(short ruleType)
        {
            //Arrange
            const int expectedRuleId = 543;

            //Act
            var actual = new UserRuleAcknowledgement("1", expectedRuleId, (RuleType)ruleType);

            //Assert
            if (((RuleType) ruleType).Equals(RuleType.GlobalRule))
            {
                actual.GlobalRuleId.Should().Be(expectedRuleId);
            }
            if (((RuleType)ruleType).Equals(RuleType.CourseRule))
            {
                actual.CourseRuleId.Should().Be(expectedRuleId);
            }
        }
    }
}
