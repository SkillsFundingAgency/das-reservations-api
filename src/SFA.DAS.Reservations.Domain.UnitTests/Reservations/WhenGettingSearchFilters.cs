using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenGettingSearchFilters
    {
        [Test]
        public void ThenWillSayIfCourseFiltersAreSelected()
        {
            //Arrange
            var filters = new SearchFilters
            {
                CourseFilters = new List<string> {"test"}
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
            var filters = new SearchFilters();

            //Act
            var result = filters.HasFilters;

            //Assert
            Assert.IsFalse(result);
        }
    }
}
