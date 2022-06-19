using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace school_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public StudentController()
        {

        }
        [HttpGet]
        public IActionResult get()
        {
            return Ok("Welcome To StudentController");
        }
    }
}
