using Microsoft.AspNetCore.Mvc;

namespace SentiMint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SentimentController : Controller
    {
        private readonly ILogger<SentimentController> _logger;

        public SentimentController(ILogger<SentimentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok("Returned Hello from SentimintController :)");
        }
    }
}
