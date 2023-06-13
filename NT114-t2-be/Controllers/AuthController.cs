using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NT114_t2_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }
    }
}
