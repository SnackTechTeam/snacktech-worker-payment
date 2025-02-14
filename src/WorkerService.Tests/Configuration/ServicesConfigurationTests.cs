using Microsoft.Extensions.Hosting;
using WorkerService.Configuration;
using WorkerService.Data.Repository;
using WorkerService.Handlers;
using WorkerService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Amazon.SQS;
using WorkerService.Configuration.Tests;
using WorkerService.Data;


namespace WorkerService.Tests.Configuration;

public class ServiceConfigurationExtensionsTests
{
    [Fact]
    public void ConfigureServices_ServicesAreAddedToCollection()
    {
        // Arrange
        var builder = new HostApplicationBuilder();
        var configOptions = AppSettingsConfigTests.MakeConfigBuilderMock();
        var config = AppSettingsConfig.LoadConfiguration(configOptions);
        builder.Services.AddSingleton(config);
        builder.ConfigureAmazonSqsService(config);
        builder.ConfigureDbContext();

        // Act
        builder.ConfigureServices();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<ISqsClient>());
        Assert.NotNull(serviceProvider.GetService<IPedidoRepository>());
        Assert.NotNull(serviceProvider.GetService<IPagamentoHandler>());
        Assert.NotNull(serviceProvider.GetService<IMensagemHandler>());
    }

    [Fact]
    public void ConfigureDbContext_DbContextIsAddedToCollection()
    {
        // Arrange
        var builder = new HostApplicationBuilder();

        // Act
        builder.ConfigureDbContext();
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<PedidoContext>());
    }

    [Fact]
    public void ConfigureAmazonSqsService_AmazonSqsServiceIsAddedToCollection()
    {
        // Arrange
        var builder = new HostApplicationBuilder();
        var configOptions = AppSettingsConfigTests.MakeConfigBuilderMock();
        var config = AppSettingsConfig.LoadConfiguration(configOptions);

        // Act
        builder.ConfigureAmazonSqsService(config);
        var serviceProvider = builder.Services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IAmazonSQS>());
    }
}