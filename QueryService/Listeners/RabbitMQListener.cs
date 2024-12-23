using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RabbitMQListener : IHostedService
{
    private readonly IDatabase _cache;
    private readonly ILogger<RabbitMQListener> _logger;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQListener(IDatabase cache, ILogger<RabbitMQListener> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Listener is starting...");

        var factory = new ConnectionFactory
        {
            HostName = "rabbitm", // Match the RabbitMQ container name
            Port = 5672,          // Default RabbitMQ port
            UserName = "guest",   // Default username
            Password = "guest"    // Default password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "mortgage-queue", durable: true, exclusive: false, autoDelete: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogInformation($"Message received: {message}");

            try
            {
                var parts = message.Split(':');
                if (parts.Length == 3)
                {
                    var key = parts[0] + ":" + parts[1];
                    var value = parts[2];
                    _cache.StringSet(key, value, TimeSpan.FromMinutes(30));
                    _logger.LogInformation($"Cache updated: {key} -> {value}");
                }
                else
                {
                    _logger.LogWarning("Invalid message format.");
                }

                // Acknowledge the message
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");

                // Reject the message and requeue it
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: "mortgage-queue", autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMQ Listener is stopping...");
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }
}