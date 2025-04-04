﻿using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Reservations.Domain.Extensions;
using SFA.DAS.Reservations.Domain.Reservations;

namespace SFA.DAS.Reservations.Domain.UnitTests.Reservations
{
    public class WhenCreatingAReservationFromIndex
    {

        [Test]
        public void ThenWillPopulatedDataFromIndexEntry()
        {
            //Arrange
            var fixture = new Fixture();
            var index = fixture.Build<ReservationIndex>()
                .Without(r => r.Id)
                .Create();

            //Act
            var reservation = index.ToReservation();

            //Assert
            reservation.Should().BeEquivalentTo(index, options => 
                options.Excluding(x => x.Id)
                       .Excluding(x => x.Status)
                       .Excluding(x => x.ReservationId)
                       .Excluding(x => x.CourseDescription)
                       .Excluding(x => x.CourseId)
                       .Excluding(x => x.CourseTitle)
                       .Excluding(x => x.CourseLevel)
                       .Excluding(x => x.IndexedProviderId));

            reservation.Id.Should().Be(index.ReservationId);
            reservation.Status.Should().Be((ReservationStatus)index.Status);
            reservation.Course.CourseId.Should().Be(index.CourseId);
            reservation.Course.Title.Should().Be(index.CourseTitle);
            reservation.Course.Level.Should().Be(index.CourseLevel.ToString());
        }

        [Test]
        public void ThenWillNotPopulatedCourseIfIdDoNotExist()
        {
            //Arrange
            var fixture = new Fixture();
            var index = fixture.Build<ReservationIndex>()
                .Without(r => r.CourseId)
                .Without(r => r.Id)
                .Create();

            //Act
            var reservation = index.ToReservation();

            //Assert
            reservation.Should().BeEquivalentTo(index, options => 
                options.Excluding(x => x.Id)
                    .Excluding(x => x.Status)
                    .Excluding(x => x.ReservationId)
                    .Excluding(x => x.CourseDescription)
                    .Excluding(x => x.CourseId)
                    .Excluding(x => x.CourseTitle)
                    .Excluding(x => x.CourseLevel)
                    .Excluding(x => x.IndexedProviderId));

            reservation.Status.Should().Be((ReservationStatus)index.Status);
            reservation.Id.Should().Be(index.ReservationId);

            reservation.Course.Should().BeNull();
        }
        
        [Test]
        public void ThenWillNotPopulatedCourseIfTitleDoNotExist()
        {
            //Arrange
            var fixture = new Fixture();
            var index = fixture.Build<ReservationIndex>()
                .Without(r => r.CourseTitle)
                .Without(r => r.Id)
                .Create();

            //Act
            var reservation = index.ToReservation();

            //Assert
            reservation.Should().BeEquivalentTo(index, options => 
                options.Excluding(x => x.Id)
                    .Excluding(x => x.Status)
                    .Excluding(x => x.ReservationId)
                    .Excluding(x => x.CourseDescription)
                    .Excluding(x => x.CourseId)
                    .Excluding(x => x.CourseTitle)
                    .Excluding(x => x.CourseLevel)
                    .Excluding(x => x.IndexedProviderId));

            reservation.Id.Should().Be(index.ReservationId);
            reservation.Status.Should().Be((ReservationStatus)index.Status);
            reservation.Course.Should().BeNull();
        }

        
        [Test]
        public void ThenWillNotPopulatedCourseIfLevelDoNotExist()
        {
            //Arrange
            var fixture = new Fixture();
            var index = fixture.Build<ReservationIndex>()
                .Without(r => r.CourseLevel)
                .Without(r => r.Id)
                .Create();

            //Act
            var reservation = index.ToReservation();

            //Assert
            reservation.Should().BeEquivalentTo(index, options => 
                options.Excluding(x => x.Id)
                    .Excluding(x => x.ReservationId)
                    .Excluding(x => x.Status)
                    .Excluding(x => x.CourseDescription)
                    .Excluding(x => x.CourseId)
                    .Excluding(x => x.CourseTitle)
                    .Excluding(x => x.CourseLevel)
                    .Excluding(x => x.IndexedProviderId));

            reservation.Status.Should().Be((ReservationStatus)index.Status);
            reservation.Id.Should().Be(index.ReservationId);

            reservation.Course.Should().BeNull();
        }
    }
}
