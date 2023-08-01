using Crawler.Infra.RabbitMq.Pub;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Crawler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessagePublisher _publisher;
        
        public MessageController(MessagePublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("publish/{queue}")]
        public IActionResult PublishToTeste([FromRoute] string queue, [FromBody] object payload)
        {
            // Process your payload and publish it
            _publisher.PublishMessageAtQueue(queue, JsonSerializer.Serialize(payload));
            return Ok();
        }
    }
}
