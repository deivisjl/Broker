using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Broker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            Dictionary<string, string> message = new Dictionary<string, string>();
            message.Add("Nombre", "Intermediario de firma MEDIIGSS-CCG");
            message.Add("Version", "1.0");

            return Ok(message);
        }
    }
}
