using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using WorkerService.Configuration;
using WorkerService.Configuration.Tests;
using WorkerService.DTOs;
using WorkerService.Handlers;
using WorkerService.Infrastructure;

namespace WorkerService.Tests.Handlers;

public class MensagemHandlerTests
{
    private readonly Mock<ILogger<MensagemHandler>> _loggerMock;
    private readonly Mock<IPagamentoHandler> _pagamentoHandlerMock;
    private readonly Mock<SqsClient> _sqsClientMock;
    private readonly AppSettingsConfig _appSettingsConfig;
    private readonly MensagemHandler _mensagemHandler;

    public MensagemHandlerTests()
    {
        _loggerMock = new Mock<ILogger<MensagemHandler>>();
        _pagamentoHandlerMock = new Mock<IPagamentoHandler>();
        
        var configuration = AppSettingsConfigTests.MakeConfigBuilderMock();
        _appSettingsConfig = AppSettingsConfig.LoadConfiguration(configuration);
        
        var amazonSqsMock = new Mock<IAmazonSQS>(); 
        _sqsClientMock = new Mock<SqsClient>(amazonSqsMock.Object);
        _sqsClientMock.Setup(sc => sc.SendMessageAsync(_appSettingsConfig.Aws.DlqQueueURL, It.IsAny<string>()))
            .Returns(Task.FromResult(new SendMessageResponse()));
        _sqsClientMock.Setup(sc => sc.DeleteMessageAsync(_appSettingsConfig.Aws.QueueURL, It.IsAny<Message>()))
            .Returns(Task.CompletedTask);

        _mensagemHandler = new MensagemHandler(_loggerMock.Object, _pagamentoHandlerMock.Object, _sqsClientMock.Object, _appSettingsConfig);
    }

    [Fact]
    public async Task Processar_MensagemValida_ProcessaMensagem()
    {
        // Arrange
        var mensagemDto = new MensagemPagamentoDto { PedidoId = Guid.NewGuid() };
        var message = new Message { Body = JsonSerializer.Serialize(mensagemDto) };

        // Act
        await _mensagemHandler.Processar(message);

        // Assert
        _pagamentoHandlerMock.Verify(ph => ph.ProcessarAsync(It.IsAny<MensagemPagamentoDto>()), Times.Once);
    }

    [Fact]
    public async Task Processar_MensagemInvalida_DeserializacaoFalha()
    {
        // Arrange
        var message = new Message { Body = "Invalid message" };

        // Act 
        await _mensagemHandler.Processar(message);
        
        // Assert mensagem enviada para DLQ
        _sqsClientMock.Verify(sc => sc.SendMessageAsync(_appSettingsConfig.Aws.DlqQueueURL, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Processar_MensagemValida_ProcessamentoFalha()
    {
        // Arrange
        var mensagemDto = new MensagemPagamentoDto { PedidoId = Guid.NewGuid() };
        var message = new Message { Body = JsonSerializer.Serialize(mensagemDto) };
        _pagamentoHandlerMock.Setup(ph => ph.ProcessarAsync(It.IsAny<MensagemPagamentoDto>())).ThrowsAsync(new Exception("Test exception"));

        // Act 
        await _mensagemHandler.Processar(message);

        // Assert enviar mensagem para DLQ
        _sqsClientMock.Verify(sc => sc.SendMessageAsync(_appSettingsConfig.Aws.DlqQueueURL, message.Body), Times.Once);
    }

    [Fact]
    public async Task Processar_MensagemValida_DeletaMensagemDaFila()
    {
        // Arrange
        var mensagemDto = new MensagemPagamentoDto { PedidoId = Guid.NewGuid() };
        var message = new Message { Body = JsonSerializer.Serialize(mensagemDto) };

        // Act
        await _mensagemHandler.Processar(message);

        // Assert
        _sqsClientMock.Verify(sc => sc.DeleteMessageAsync(_appSettingsConfig.Aws.QueueURL, message), Times.Once);
    }

    [Fact]
    public async Task Processar_MensagemInvalida_DeletaMensagemDaFila()
    {
        // Arrange
        var message = new Message { Body = "Invalid message" }; 

        // Act
        await _mensagemHandler.Processar(message);

        // Assert
        _sqsClientMock.Verify(sc => sc.DeleteMessageAsync(_appSettingsConfig.Aws.QueueURL, It.IsAny<Message>()), Times.Once);
    }
}