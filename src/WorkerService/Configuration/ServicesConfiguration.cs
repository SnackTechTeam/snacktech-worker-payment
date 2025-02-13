using WorkerService.Handlers;
using WorkerService.Infrastructure;
using Amazon.SQS;
using Amazon;
using WorkerService.Data;
using Microsoft.EntityFrameworkCore;
using WorkerService.Data.Repository;


namespace WorkerService.Configuration;

public static class ServiceConfigurationExtensions
{    
    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ISqsClient, SqsClient>();
        builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
        builder.Services.AddScoped<IPagamentoHandler, PagamentoHandler>();
        builder.Services.AddScoped<IMensagemHandler, MensagemHandler>();

        return builder;
    }

    public static HostApplicationBuilder ConfigureDbContext(this HostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<PedidoContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

        return builder;
    }

    public static HostApplicationBuilder ConfigureAmazonSqsService(this HostApplicationBuilder builder, AppSettingsConfig config)
    {
        builder.Services.AddSingleton<IAmazonSQS>(serviceProvider =>
        {
            var configSqs = new AmazonSQSConfig
            {
                ServiceURL = config.Aws.ServiceURL
            };
            return new AmazonSQSClient(config.Aws.AccessKey, config.Aws.SecretKey, configSqs);
        });

        return builder;
    }
}