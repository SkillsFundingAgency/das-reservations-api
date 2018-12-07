using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Reservations.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        public async Task<IActionResult> Index()
        {
            return Ok("Success");
        }
        
    }
}
