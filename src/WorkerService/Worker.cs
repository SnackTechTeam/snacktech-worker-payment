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
            var handler = services.GetService<IPagamentoHandler>();

            while (!stoppingToken.IsCancellationRequested)
            {
                
                var response = await _sqsClient.ReceiveMessageAsync(_config.Aws.QueueURL);

                foreach (var message in response.Messages)
                {
                    _logger.LogDebug("Processando mensagem com id: {messageId}", message?.MessageId);

                    try
                    {
                        var mensagemDto = JsonSerializer.Deserialize<MensagemPagamentoDto>(message!.Body);
                        if(mensagemDto == null)
                        {
                            throw new JsonException("Erro ao deserializar mensagem");
                        }

                        await handler!.ProcessarAsync(mensagemDto);
                        _logger.LogDebug("Mensagem processada com sucesso com id: {messageId} e body: {body}", message?.MessageId, message?.Body);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar mensagem com id: {messageId} e body: {body}", message?.MessageId, message?.Body);
                        
                        if (message != null){
                            await _sqsClient.SendMessageAsync(_config.Aws.DlqQueueURL, message.Body);
                            _logger.LogWarning("Mensagem com id: {messageId} enviada para DLQ {dlqQueueURL}", message.MessageId, _config.Aws.DlqQueueURL);
                        }
                    }
                    finally
                    {
                        if (message != null){
                            await _sqsClient.DeleteMessageAsync(_config.Aws.QueueURL, message);
                            _logger.LogDebug("Mensagem com id: {messageId} deletada da fila {queueURL}", message.MessageId, _config.Aws.QueueURL);
                        }
                    }
                }

                _logger.LogDebug("Aguardando novas mensagens...");
                await Task.Delay(5000);
            }

            scope.Dispose();
        }
    }
}