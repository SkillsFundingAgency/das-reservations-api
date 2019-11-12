using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Data.ElasticSearch;

namespace SFA.DAS.Reservations.Data.UnitTests.ElasticSearch
{
    public class WhenHandlingAnElasticResponse
    {
        [Test]
        public void ThenWillShowSearchHitsAsItems()
        {
            //Arrange
            var expectedResponseHits = new List<string> {"1", "2", "3"};

            var response = new ElasticResponse<string>();
            response.hits = new Hits<string>()
            {
                hits = expectedResponseHits.Select(h => new Hit<string> {_source = h}).ToList()
            };

            //Act
            var responseItems = response.Items;

            //Assert
            responseItems.Should().BeEquivalentTo(expectedResponseHits);
        }

        [Test]
        public void ThenWillReturnEmptyListIfNoHitsExist()
        {
            //Arrange
            var expectedResponseHits = new List<string> {"1", "2", "3"};

            var response = new ElasticResponse<string>();
            response.hits = new Hits<string>();

            //Act
            var responseItems = response.Items;

            //Assert
            responseItems.Should().NotBeNull();
            responseItems.Should().BeEmpty();
        }

        [Test]
        public void ThenWillReturnEmptyListIfHitsIsNull()
        {
            //Arrange
            var response = new ElasticResponse<string>();

            //Act
            var responseItems = response.Items;

            //Assert
            responseItems.Should().NotBeNull();
            responseItems.Should().BeEmpty();
        }

    }
}
