using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class MortgageController : ControllerBase
{
    [HttpPost("send")]
    public IActionResult SendMessage()
    {
        var factory = new ConnectionFactory
        {
            HostName = "rabbitm",
            Port = 5672  // Default RabbitMQ port
        };

        // Explicitly specify the connection name
        using var connection = factory.CreateConnection("MortgageConnection");
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: "mortgage-queue",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        string message = "Mortgage pricing update";
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "",
                             routingKey: "mortgage-queue",
                             basicProperties: null,
                             body: body);

        return Ok("Message sent to RabbitMQ v7!");
    }
}