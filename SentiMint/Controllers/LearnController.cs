using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace SentiMint.Controllers
{
    [Route("api/[controller]")]
    public class LearnController : Controller
    {
        private readonly ILogger<LearnController> _logger;

        public LearnController(ILogger<LearnController> logger)
        {
            _logger = logger;
        }

        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok("Returned Hello from API :)");
        }

        [HttpGet("Greet")]
        public IActionResult GreetUser([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest($"Invalid input :)");
            }
            return Ok($"Hey {name}, the SRK");
        }

        public class Info
        {
            public string Name { get; set; }
            public string WorksAt { get; set; }
            public string Location { get; set; }
        }

        readonly Info user = new Info { Name = "Raj", WorksAt = "MSFT", Location = "BLR" };

        [HttpPost("submit")]
        public IActionResult SubmitData([FromBody] Info data)
        {
            if (data == null)
            {
                return BadRequest("No data provided.");
            }
            // Persist Info object in DB.
            _logger.LogInformation($"Received data: Name: {data.Name}, WorksAt: {data.WorksAt}, Location: {data.Location}");
            return Ok($"Data received!!");
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateUser([FromBody] Info info, [DisallowNull] int id)
        {
            if(info == null || id < 0)
            {
                return BadRequest("Invalid data :(");
            }
            // Persist update operation to DB.
            _logger.LogInformation($"Updated user: {id}");
            return Ok($"User {id} updated successfully!!");
        }

        [HttpPatch("update/{id}")]
        public IActionResult PatchUser([FromBody] Info info, [DisallowNull] int id)
        {
            if(info == null || id < 0)
            {
                return BadRequest("Invalid data :(");
            }
            // Persist update operation to DB.
            _logger.LogInformation($"Patched user: {id}");
            return Ok($"User {id} patched successfully!!");
        }
    }
}
