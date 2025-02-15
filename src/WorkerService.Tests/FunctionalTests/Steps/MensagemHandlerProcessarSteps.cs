using System.Text.Json;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Moq;
using TechTalk.SpecFlow;
using WorkerService.Configuration;
using WorkerService.Configuration.Tests;
using WorkerService.DTOs;
using WorkerService.Handlers;
using WorkerService.Infrastructure;

namespace SnackTech.Products.Driver.API.Tests.ControllersFunctionalTests;

[Binding]
public class MensagemHandlerProcessarSteps
{
    MensagemHandler mensagemHandler;
    private readonly Mock<ISqsClient> _sqsClientMock;
    private readonly Mock<ILogger<MensagemHandler>> _loggerMock;  
    private readonly Mock<IPagamentoHandler> _pagamentoHandlerMock;
    private readonly AppSettingsConfig _appSettingsConfig;
    private Message? _message;
    
    public MensagemHandlerProcessarSteps()
    {
        _sqsClientMock = new Mock<ISqsClient>();
        _loggerMock = new Mock<ILogger<MensagemHandler>>();
        _pagamentoHandlerMock = new Mock<IPagamentoHandler>();
        var configuration = AppSettingsConfigTests.MakeConfigBuilderMock();
        _appSettingsConfig = AppSettingsConfig.LoadConfiguration(configuration);
        _message = null!;
        mensagemHandler = new MensagemHandler(_loggerMock.Object, _pagamentoHandlerMock.Object, _sqsClientMock.Object, _appSettingsConfig);    
    }

    [Given(@"o pedido é atualizado com sucesso")]
    public void DadoOPedidoEAtualizadoComSucesso()
    {
        _pagamentoHandlerMock.Setup(ph => ph.ProcessarAsync(It.IsAny<MensagemPagamentoDto>())).ReturnsAsync(true);
    }

    [When(@"uma mensagem válida é processada")]
    public async Task QuandoEuProcessoUmaMensagemValida()
    {
        var mensagemDto = new MensagemPagamentoDto { PedidoId = Guid.NewGuid(), PagamentoId = Guid.NewGuid(), DataRecebimento = DateTime.Now };
        _message = new Message { Body = JsonSerializer.Serialize(mensagemDto), MessageId = Guid.NewGuid().ToString() };
        await mensagemHandler.Processar(_message);
    }

    [Then(@"a mensagem deve ser excluída da fila")]
    public void EntaoAMensagemDeveSerExcluidaDaFila()
    {
        _sqsClientMock.Verify(sc => sc.DeleteMessageAsync(_appSettingsConfig.Aws.QueueURL, _message!), Times.Once);
    }

    [Then(@"a mensagem não é enviada para a DLQ")]
    public void EntaoAMensagemNaoDeveSerEnviadaParaDLQ()
    {
        _sqsClientMock.Verify(sc => sc.SendMessageAsync(_appSettingsConfig.Aws.DlqQueueURL, _message!.Body), Times.Never);
    }

    [Then(@"a mensagem é enviada para a DLQ")]
    public void EntaoAMensagemDeveSerEnviadaParaDLQ()
    {
        _sqsClientMock.Verify(sc => sc.SendMessageAsync(_appSettingsConfig.Aws.DlqQueueURL, _message!.Body), Times.Once);
    }

    [Given(@"pedido falha ao ter o pagamento atualizado")]
    public void DadoPedidoFalhaAoTerOPagamentoAtualizado()
    {
        _pagamentoHandlerMock.Setup(ph => ph.ProcessarAsync(It.IsAny<MensagemPagamentoDto>())).ReturnsAsync(false);
    }

    [When(@"uma mensagem inválida é processada")]
    public async Task QuandoEuProcessoUmaMensagemInvalida()
    {
        _message = new Message { Body = "Invalid message", MessageId = Guid.NewGuid().ToString() };
        await mensagemHandler.Processar(_message);
    }
}