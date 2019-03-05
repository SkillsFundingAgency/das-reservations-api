using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Reservations.Api.Controllers;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;

namespace SFA.DAS.Reservations.Api.UnitTests.Controllers.Courses
{
    public class WhenGettingCourses
    {
        private CoursesController _coursesController;
        private Mock<IMediator> _mediator;
        private GetCoursesResponse _coursesResponse;
        private const long ExpectedAccountId = 123234;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _coursesResponse = new GetCoursesResponse{Courses = new List<Domain.Courses.Course>()};
            
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
            Assert.IsNotNull(actual);
            var result = actual as ObjectResult;
            Assert.IsNotNull(result?.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.IsNotNull(result.Value);
            var actualCourses = result.Value as List<Domain.Courses.Course>;
            Assert.AreEqual(_coursesResponse.Courses,actualCourses);
        }
    }
}
