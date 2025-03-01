using WorkerService.Infrastructure;
using WorkerService.Handlers;
using WorkerService.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace WorkerService
{
    [ExcludeFromCodeCoverage]
    public class Worker(ILogger<Worker> logger,ISqsClient sqsClient,AppSettingsConfig config,IServiceProvider serviceProvider) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService<IMensagemHandler>();
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await sqsClient.ReceiveMessageAsync(config.Aws.QueueURL);

                foreach (var message in response.Messages) await handler!.Processar(message);

                logger.LogDebug("Aguardando novas mensagens...");
                await Task.Delay(5000);
            }

            scope.Dispose();
        }
    }
}