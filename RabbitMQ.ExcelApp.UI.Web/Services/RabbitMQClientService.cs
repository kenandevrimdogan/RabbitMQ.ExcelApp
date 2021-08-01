using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;

namespace RabbitMQ.ExcelApp.UI.Web.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static readonly string ExchangeName = "ExcelDirectExchange";
        public static readonly string RoutingExcelName = "excel-route-file";
        private static readonly string QueueName = "queue-excel-file";

        private readonly ILogger<RabbitMQClientService> _logger;

        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }


        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();

            if (_channel is { IsOpen: true })
            {
                return _channel;
            }

            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct, true, false);
            _channel.QueueDeclare(QueueName, true, false, false, null);
            _channel.QueueBind(QueueName, ExchangeName, routingKey: RoutingExcelName);

            _logger.LogInformation("RabbitMQ Connect");

            return _channel;
        }

        public void Dispose()
        {
            _channel.Close();
            _channel?.Dispose();

            _connection.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabitMQ Disconnected");
        }
    }
}
