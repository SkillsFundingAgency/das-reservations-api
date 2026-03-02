using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Rules;

namespace SFA.DAS.Reservations.Domain.UnitTests.Rules;

public class WhenGettingAvailableDates
{
    private DateTime _expectedMinStartDate;
    private DateTime _expectedMaxStartDate;

    private const int ExpectedExpiryPeriod = 10;

    [SetUp]
    public void Arrange()
    {
        _expectedMinStartDate = DateTime.UtcNow;
        _expectedMaxStartDate = DateTime.UtcNow.AddMonths(3);
    }

    [Test]
    public void Then_The_Available_Dates_Are_Returned_Based_On_The_Configurable_Months_Allowed()
    {
        // Arrange
        const int numberOfAvailableDatesIncludingPreviousMonth = ExpectedExpiryPeriod + 1;
        var previousMonthDate = DateTime.UtcNow.AddMonths(-1);
            
        // Act
        var actual = new AvailableDates(DateTime.UtcNow, ExpectedExpiryPeriod);

        // Assert
        actual.Dates.Count.Should().Be(numberOfAvailableDatesIncludingPreviousMonth);
        actual.Dates.First().StartDate.ToString("MMyyyy").Should().Be(previousMonthDate.ToString("MMyyyy"));
        actual.Dates.Last().StartDate.ToString("MMyyyy").Should().Be(DateTime.UtcNow.AddMonths(ExpectedExpiryPeriod - 1).ToString("MMyyyy"));
        actual.Dates.First().EndDate.ToString("MMyyyy").Should().Be(previousMonthDate.AddMonths(2).ToString("MMyyyy"));

    }

    [Test]
    public void Then_If_The_Min_Date_Is_Set_The_First_Month_Starts_From_There()
    {
        //Act
        var actual = new AvailableDates(DateTime.UtcNow, minStartDate: _expectedMinStartDate);

        //Assert
        actual.Dates.First().StartDate.ToString("MMyyyy").Should().Be(_expectedMinStartDate.ToString("MMyyyy"));
    }

    [Test]
    public void Then_If_The_Max_Date_Is_Set_The_Available_Date_Does_Not_Exceed_This_Value_From_The_Configurable_Number_Of_Months()
    {
        //Act
        var actual = new AvailableDates(DateTime.UtcNow, maxStartDate:_expectedMaxStartDate);

        //Assert
        actual.Dates.Count.Should().Be(4);
        actual.Dates.First().StartDate.ToString("MMyyyy").Should().Be(DateTime.UtcNow.ToString("MMyyyy"));
        actual.Dates.Last().StartDate.ToString("MMyyyy").Should().Be(_expectedMaxStartDate.ToString("MMyyyy"));
    }

    [Test]
    public void And_Today_Is_Same_As_Start_And_End_Date()
    {
        //Act
        var actual = new AvailableDates(DateTime.UtcNow, minStartDate: DateTime.UtcNow, maxStartDate: DateTime.UtcNow);

        //Assert
        actual.Dates.Count.Should().Be(1);
    }

    [Test]
    public void And_Today_Is_Same_As_Start_And_One_Month_Expiry()
    {
        //Act
        var actual = new AvailableDates(DateTime.UtcNow, minStartDate: DateTime.UtcNow, expiryPeriodInMonths:1);

        //Assert
        actual.Dates.Count.Should().Be(1);
    }

    [Test]
    public void Then_If_No_Values_Have_Been_Supplied_Then_The_Default_Of_Six_Months_Are_Shown_From_Today()
    {
        //Arrange
        const int expiryDefault = 6;
            
        //Act
        var actual = new AvailableDates(DateTime.UtcNow);

        //Assert
        const int expectedMonthsIncludingPrevious = expiryDefault + 1;
        actual.Dates.Count.Should().Be(expectedMonthsIncludingPrevious);
        actual.Dates.First().StartDate.ToString("MMyyyy").Should().Be(DateTime.UtcNow.AddMonths(-1).ToString("MMyyyy"));
        actual.Dates.Last().StartDate.ToString("MMyyyy").Should().Be(DateTime.UtcNow.AddMonths(expiryDefault - 1).ToString("MMyyyy"));
    }

    [Test]
    public void Then_The_Start_And_End_Window_Is_Set_To_Three_Months()
    {
        //Act
        var actual = new AvailableDates(DateTime.UtcNow);

        //Assert
        actual.Dates
            .All(c => (c.EndDate.Year - c.StartDate.Year) * 12 + c.EndDate.Month - c.StartDate.Month == 2)
            .Should()
            .BeTrue();
    }

    [Test]
    public void Then_The_Dates_Are_Set_To_The_First_Of_The_Month_And_The_EndDate_To_The_Last()
    { 
        //Act
        var actual = new AvailableDates(DateTime.UtcNow);

        //Assert
        actual.Dates.All(c => c.StartDate.Day.Equals(1)).Should().BeTrue();
        actual.Dates.All(c=>c.EndDate.Day.Equals(DateTime.DaysInMonth(c.EndDate.Year,c.EndDate.Month))).Should().BeTrue();
    }

    [Test]
    public void Then_Only_A_Maximum_Of_Twelve_Available_Dates_Plus_Previous_Month_Will_Be_Returned()
    {
        //Act
        var actual = new AvailableDates(DateTime.UtcNow,20);

        //Actual
        actual.Dates.Count.Should().Be(13);
    }

    [Test]
    public void Then_When_IncludePreviousMonth_Is_False_The_First_Date_Is_Current_Month()
    {
        var now = new DateTime(2025, 6, 15);
        var actual = new AvailableDates(now, 6, null, null, includePreviousMonth: false);

        actual.Dates.Should().NotBeEmpty();
        actual.Dates.First().StartDate.Year.Should().Be(2025);
        actual.Dates.First().StartDate.Month.Should().Be(6);
        actual.Dates.First().StartDate.Day.Should().Be(1);
    }
}