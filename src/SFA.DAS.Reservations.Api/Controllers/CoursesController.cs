using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Reservations.Application.Courses.Queries.GetCourses;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetCoursesQuery( ));
            return Ok(response.Courses);
        }
    }

}
