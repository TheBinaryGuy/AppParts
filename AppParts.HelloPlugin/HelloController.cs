using System;
using Microsoft.AspNetCore.Mvc;

namespace AppParts.HelloPlugin
{
    public class HelloController : Controller
    {
        public IActionResult Greet(string greetTo)
        {
            return View(model: greetTo);
        }
    }
}
