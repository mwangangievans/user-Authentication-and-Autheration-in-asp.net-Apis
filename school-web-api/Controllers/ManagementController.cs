using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace school_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        public ManagementController()
        {

        }
        [HttpGet]
        public IActionResult get()
        {
            return Ok("welcome To ManagementController");
        }
    }
}
