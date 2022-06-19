using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace school_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
    
        public HomeController()
        {

        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome Home Controller");
        }
    }
}
