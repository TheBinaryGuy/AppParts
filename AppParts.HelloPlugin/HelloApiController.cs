using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace AppParts.HelloPlugin
{
    [ApiController]
    public class HelloApiController : ControllerBase
    {
        [HttpGet("api-greet")]
        public IActionResult Greet(string greetTo)
        {
            return Ok(new { greet = greetTo.Humanize() });
        }
    }
}
