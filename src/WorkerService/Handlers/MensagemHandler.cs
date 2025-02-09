using System.Text.Json;
using Amazon.SQS.Model;
using WorkerService.Configuration;
using WorkerService.DTOs;
using WorkerService.Infrastructure;

namespace WorkerService.Handlers
{
    public class MensagemHandler : IMensagemHandler
    {
        private readonly ILogger<MensagemHandler> logger;
        private readonly IPagamentoHandler handler;
        private readonly ISqsClient sqsClient;
        private readonly AppSettingsConfig config;

        public MensagemHandler(ILogger<MensagemHandler> logger, IPagamentoHandler handler, ISqsClient sqsClient, AppSettingsConfig config)
        {
            this.logger = logger;
            this.handler = handler;
            this.sqsClient = sqsClient;
            this.config = config;
        }
        
        public async Task Processar(Message mensagem)
        {
            logger.LogDebug("Processando mensagem com id: {messageId}", mensagem?.MessageId);

            try
            {
                var mensagemDto = JsonSerializer.Deserialize<MensagemPagamentoDto>(mensagem!.Body);
                if(mensagemDto == null)
                {
                    throw new JsonException("Erro ao deserializar mensagem");
                }

                await handler!.ProcessarAsync(mensagemDto);
                logger.LogDebug("Mensagem processada com sucesso com id: {messageId} e body: {body}", mensagem?.MessageId, mensagem?.Body);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem com id: {messageId} e body: {body}", mensagem?.MessageId, mensagem?.Body);
                
                if (mensagem != null){
                    await sqsClient.SendMessageAsync(config.Aws.DlqQueueURL, mensagem.Body);
                    logger.LogWarning("Mensagem com id: {messageId} enviada para DLQ {dlqQueueURL}", mensagem.MessageId, config.Aws.DlqQueueURL);
                }
            }
            finally
            {
                if (mensagem != null){
                    await sqsClient.DeleteMessageAsync(config.Aws.QueueURL, mensagem);
                    logger.LogDebug("Mensagem com id: {messageId} deletada da fila {queueURL}", mensagem.MessageId, config.Aws.QueueURL);
                }
            }
        }
    }
}