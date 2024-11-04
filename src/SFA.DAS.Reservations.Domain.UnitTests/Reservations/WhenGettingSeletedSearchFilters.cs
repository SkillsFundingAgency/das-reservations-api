using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenGettingSeletedSearchFilters
    {
        [Test]
        public void ThenWillSayIfCourseFiltersAreSelected()
        {
            //Arrange
            var filters = new SelectedSearchFilters
            {
                CourseFilter = "Test"
            };

            //Act
            var result = filters.HasFilters;

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ThenWillSayIfEmployerFiltersAreSelected()
        {
            //Arrange
            var filters = new SelectedSearchFilters
            {
                EmployerNameFilter = "Test"
            };

            //Act
            var result = filters.HasFilters;

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ThenWillSayIfStartDateFiltersAreSelected()
        {
            //Arrange
            var filters = new SelectedSearchFilters
            {
                StartDateFilter = DateTime.Now.ToString("g")
            };

            //Act
            var result = filters.HasFilters;

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void ThenWillSayIfFiltersAreNotSelected()
        {
            //Arrange
            var filters = new SelectedSearchFilters();

            //Act
            var result = filters.HasFilters;

            //Assert
            result.Should().BeFalse();
        }
    }
}
