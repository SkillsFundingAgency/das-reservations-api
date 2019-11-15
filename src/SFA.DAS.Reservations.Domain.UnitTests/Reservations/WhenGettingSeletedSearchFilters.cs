using System.Collections.Generic;
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
            Assert.IsTrue(result);
        }

        [Test]
        public void ThenWillSayIfFiltersAreNotSelected()
        {
            //Arrange
            var filters = new SelectedSearchFilters();

            //Act
            var result = filters.HasFilters;

            //Assert
            Assert.IsFalse(result);
        }
    }
}
