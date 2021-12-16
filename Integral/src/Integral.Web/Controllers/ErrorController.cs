using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Integral.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int? code)
        {
            return View();
        }
    }
}
