using Microsoft.AspNetCore.Mvc;

namespace HealthCheckController.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {

        [HttpGet]
        public ActionResult Check()
        {
            return Ok("alive");
        }
    }
}