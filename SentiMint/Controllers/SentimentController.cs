using Microsoft.AspNetCore.Mvc;
using SentiMint.Models;
using SentiMint.Services;

namespace SentiMint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SentimentController : Controller
    {
        private readonly ILogger<SentimentController> _logger;

        private readonly SentimentAnalysisEngine _sentimentAnalysisEngine;

        public SentimentController(ILogger<SentimentController> logger, SentimentAnalysisEngine sentimentAnalysisEngine)
        {
            _logger = logger;
            _sentimentAnalysisEngine = sentimentAnalysisEngine;
        }

        [HttpPost("predict")]
        public IActionResult PredictSentiment([FromBody] SentiMintRequest request)
        { 
            if(string.IsNullOrEmpty(request?.ReviewText))
            {
                return BadRequest("Review text is required");
            }

            var prediction = _sentimentAnalysisEngine.Predict(request.ReviewText);
            return Ok(prediction);
        }
    }
}
