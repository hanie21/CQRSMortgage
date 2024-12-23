using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;

[Route("api/[controller]")]
[ApiController]
public class CacheController : ControllerBase
{
    private readonly IDatabase _cache;
    private readonly ILogger<CacheController> _logger;

    public CacheController(ILogger<CacheController> logger)
    {
        _logger = logger;

        // Explicit Redis connection configuration
        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { "redis:6379" }, // Redis host and port
            AbortOnConnectFail = false,      // Retry on failure
            ConnectRetry = 5,                // Retry 5 times
            ConnectTimeout = 5000,           // 5 seconds timeout
            KeepAlive = 180                  // Keep connection alive for 3 minutes
        };

        var redis = ConnectionMultiplexer.Connect(configurationOptions);
        _cache = redis.GetDatabase();
    }

    /// <summary>
    /// Retrieves a value from the cache for a given key.
    /// </summary>
    [HttpGet("{key}")]
    public IActionResult Get([Required] string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogError("Key cannot be empty.");
            return BadRequest(new { Error = "Key cannot be empty." });
        }
        try
        {
            var value = _cache.StringGet(key);
            if (value.IsNullOrEmpty)
                return NotFound(new { Error = $"No value found for key: {key}" });

            return Ok(new { Key = key, Value = value.ToString() });
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError("Redis connection failed.");
            return StatusCode(500, new { Error = "Redis connection failed.", Details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error retrieving data.", Details = ex.Message });
        }
    }

    /// <summary>
    /// Stores a key-value pair in the cache.
    /// </summary>
    [HttpPost]
    public IActionResult Set([FromBody] CacheRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Key) || string.IsNullOrWhiteSpace(request.Value))
            return BadRequest(new { Error = "Key and Value are required." });

        try
        {
            // Set value with a Time-To-Live (TTL) of 30 minutes
            bool isSet = _cache.StringSet(request.Key, request.Value, expiry: TimeSpan.FromMinutes(30));
            if (!isSet)
            {
                return StatusCode(500, new { Error = "Failed to cache data." });
            }

            return Ok(new { Message = "Data cached successfully!", request.Key, request.Value });
        }
        catch (RedisConnectionException ex)
        {
            return StatusCode(500, new { Error = "Redis connection failed.", Details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Error saving data.", Details = ex.Message });
        }
    }
}

// DTO for the POST request
public class CacheRequest
{
    [Required(ErrorMessage = "Key is required.")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Value is required.")]
    public string Value { get; set; } = string.Empty;
}