using WorkerService.DTOs;
using WorkerService.Infrastructure;
using WorkerService.Handlers;
using System.Text.Json;
using WorkerService.Configuration;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISqsClient _sqsClient;
        private readonly AppSettingsConfig _config;
        private readonly IServiceProvider _serviceProvider;

        public Worker(
            ILogger<Worker> logger,
            ISqsClient sqsClient,
            AppSettingsConfig config,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sqsClient = sqsClient;
            _config = config;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var handler = services.GetService<IPedidoHandler>();

            while (!stoppingToken.IsCancellationRequested)
            {
                
                var response = await _sqsClient.ReceiveMessageAsync(_config.Aws.QueueName);

                foreach (var message in response.Messages)
                {
                    _logger.LogDebug("Processing message {messageId}", message?.MessageId);

                    try
                    {
                        var mensagemPedidoDto = JsonSerializer.Deserialize<MensagemPedidoDto>(message!.Body);
                        if(mensagemPedidoDto == null)
                        {
                            throw new JsonException("Invalid message body");
                        }

                        await handler!.ProcessarPedidoAsync(mensagemPedidoDto);
                        _logger.LogDebug("Message {messageId} processed with body: {body}", message?.MessageId, message?.Body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message {messageId} with body: {body}", message?.MessageId, message?.Body);
                        
                        if (message != null){
                            await _sqsClient.SendMessageAsync(_config.Aws.DlqQueueName, message.Body);
                            _logger.LogWarning("Message {messageId} sent to DLQ {queueDlq}", message.MessageId, _config.Aws.DlqQueueName);
                        }
                    }
                    finally
                    {
                        if (message != null){
                            await _sqsClient.DeleteMessageAsync(_config.Aws.QueueName, message);
                            _logger.LogDebug("Message {messageId} deleted from queue {queue}", message.MessageId, _config.Aws.QueueName);
                        }
                    }
                }

                _logger.LogDebug("Waiting for messages...");
                await Task.Delay(5000);
            }

            scope.Dispose();
        }
    }
}