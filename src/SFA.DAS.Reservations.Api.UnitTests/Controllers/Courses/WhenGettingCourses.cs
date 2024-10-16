using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Courses
{
    public class WhenGettingCourses
    {
        private CoursesController _coursesController;
        private Mock<IMediator> _mediator;
        private GetCoursesResponse _coursesResponse;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _coursesResponse = new GetCoursesResponse{Courses = new List<Domain.ApprenticeshipCourse.Course>()};
            
            _mediator.Setup(x => x.Send(It.IsAny<GetCoursesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_coursesResponse);

            _coursesController = new CoursesController(_mediator.Object);
        }

        [Test]
        public async Task Then_The_Courses_Are_Returned()
        {
            //Act
            var actual = await _coursesController.GetAll();

            //Assert
            actual.Should().NotBeNull();

            var result = actual.Should().BeOfType<ObjectResult>().Subject;
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Value.Should().NotBeNull();

            var viewModel = result.Value.Should().BeOfType<CoursesViewModel>().Subject;
            viewModel.Courses.Should().BeEquivalentTo(_coursesResponse.Courses);
        }
    }
}
