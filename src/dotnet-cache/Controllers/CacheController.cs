using Microsoft.AspNetCore.Mvc;
using System.Runtime.Caching;

namespace DotNetCacheAPI.Controllers
{
    [ApiController]
    [Route("cache")]
    public class CacheController : ControllerBase
    {
        private readonly ObjectCache _cache;
        private readonly ILogger<CacheController> _logger;

        public CacheController(ILogger<CacheController> logger)
        {
            _cache = MemoryCache.Default;
            _logger = logger;
        }

        public class SetValueRequest
        {
            public string? Value { get; set; }
        }

        // POST cache/set?duration=seconds
        [HttpPost("set")]
        public IActionResult Set([FromBody] SetValueRequest request, [FromQuery] int? duration = 300)
        {
            if (string.IsNullOrEmpty(request.Value))
            {
                return BadRequest("Value is required.");
            }

            var key = Guid.NewGuid().ToString();

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(duration ?? 300) // 5 minutes default duration
            };

            _cache.Set(key, request.Value, policy);

            _logger.LogInformation($"Value stored with key '{key}' for {duration} seconds");
            return Ok(new {key = key, duration = duration});
        }

        // GET cache/get/key
        [HttpGet("get/{key}")]
        public IActionResult Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Key is required.");
            }

            var value = _cache.Get(key);

            if (value == null)
            {
                _logger.LogWarning($"No value found for key '{key}'");
                return NotFound($"No value found for key '{key}'");
            }

            _logger.LogInformation($"Value retrieved with key '{key}'");
            return Ok(new { value = value.ToString() });
        }

        // GET cache/count
        [HttpGet("count")]
        public IActionResult Count()
        {
            var count = _cache.Count();

            _logger.LogInformation($"{count} items in cache");

            return Ok(count);
        }

        // DELETE cache/remove/key
        [HttpDelete("remove/{key}")]
        public IActionResult Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest("Key is required.");
            }

            var value = _cache.Remove(key);

            if (value == null)
            {
                _logger.LogWarning($"No value found for key '{key}'");
                return NotFound($"No value found for key '{key}'");
            }

            _logger.LogInformation($"Value removed with key '{key}'");
            return Ok($"Value removed with key '{key}'");
        }
    }
}