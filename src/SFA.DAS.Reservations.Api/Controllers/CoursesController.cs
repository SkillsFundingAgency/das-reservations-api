using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class CoursesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await mediator.Send(new GetCoursesQuery());

            var viewModel = new CoursesViewModel(response.Courses);

            return Ok(viewModel);
        }
    }
}