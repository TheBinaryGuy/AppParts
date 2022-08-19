using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace AppParts.HelloPlugin
{
    public class HelloController : Controller
    {
        [HttpGet("greet")]
        public IActionResult Greet(string greetTo) => View(model: greetTo);
    }
}
