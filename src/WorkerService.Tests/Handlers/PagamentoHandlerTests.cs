using Xunit;
using Moq;
using System.Threading.Tasks;
using WorkerService.Data.Entities;
using WorkerService.Data.Repository;
using WorkerService.DTOs;
using WorkerService.Handlers;
using Microsoft.Extensions.Logging;

namespace WorkerService.Tests.Handlers
{
    public class PagamentoHandlerTests
    {
        private readonly Mock<IPedidoRepository> _pedidoRepositoryMock;
        private readonly PagamentoHandler _pagamentoHandler;

        public PagamentoHandlerTests()
        {
            _pedidoRepositoryMock = new Mock<IPedidoRepository>();
            var loggerMock = new Mock<ILogger<PagamentoHandler>>();
            _pagamentoHandler = new PagamentoHandler(loggerMock.Object, _pedidoRepositoryMock.Object);
        }

        [Fact]
        public async Task ProcessarAsync_PedidoNotFound_ReturnFalse()
        {
            // Arrange
            var mensagem = new MensagemPagamentoDto { PedidoId = Guid.NewGuid() };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(mensagem.PedidoId)).ReturnsAsync((Pedido?)null);

            // Act and Assert
            Assert.False(await _pagamentoHandler.ProcessarAsync(mensagem));
        }

        [Fact]
        public async Task ProcessarAsync_PedidoStatusNotAguardandoPagamento_ReturnFalse()
        {
            // Arrange
            var mensagem = new MensagemPagamentoDto { PedidoId = Guid.NewGuid() };
            var pedido = new Pedido { Id = mensagem.PedidoId, Status = 1 };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(mensagem.PedidoId)).ReturnsAsync(pedido);

            // Act and Assert
            Assert.False(await _pagamentoHandler.ProcessarAsync(mensagem));
        }

        [Fact]
        public async Task ProcessarAsync_PedidoPagamentoIdAlreadySet_ReturnFalse()
        {
            // Arrange
            var mensagem = new MensagemPagamentoDto { PedidoId = Guid.NewGuid() };
            var pedido = new Pedido { Id = mensagem.PedidoId, Status = PagamentoHandler.STATUS_AGUARDANDO_PAGAMENTO, PagamentoId = Guid.NewGuid() };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(mensagem.PedidoId)).ReturnsAsync(pedido);

            // Act and Assert
            Assert.False(await _pagamentoHandler.ProcessarAsync(mensagem));
        }

        [Fact]
        public async Task ProcessarAsync_MensagemDataRecebimentoBeforePedidoDataCriacao_ReturnFalse()
        {
            // Arrange
            var mensagem = new MensagemPagamentoDto { PedidoId = Guid.NewGuid(), DataRecebimento = DateTime.Now.AddDays(-1) };
            var pedido = new Pedido { Id = mensagem.PedidoId, Status = PagamentoHandler.STATUS_AGUARDANDO_PAGAMENTO, DataCriacao = DateTime.Now };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(mensagem.PedidoId)).ReturnsAsync(pedido);

            // Act and Assert
            Assert.False(await _pagamentoHandler.ProcessarAsync(mensagem));
        }

        [Fact]
        public async Task ProcessarAsync_ValidMessage_ProcessesPayment()
        {
            // Arrange
            var mensagem = new MensagemPagamentoDto { PedidoId = Guid.NewGuid(), DataRecebimento = DateTime.Now };
            var pedido = new Pedido { Id = mensagem.PedidoId, Status = PagamentoHandler.STATUS_AGUARDANDO_PAGAMENTO, DataCriacao = DateTime.Now.AddDays(-1) };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(mensagem.PedidoId)).ReturnsAsync(pedido);

            // Act
            await _pagamentoHandler.ProcessarAsync(mensagem);

            // Assert
            Assert.Equal(PagamentoHandler.STATUS_PAGAMENTO_REALIZADO, pedido.Status);
            Assert.Equal(mensagem.PagamentoId, pedido.PagamentoId);
            _pedidoRepositoryMock.Verify(r => r.UpdateAsync(pedido), Times.Once);
        }

        [Fact]
        public async Task ProcessarAsync_InvalidMessage_PedidoJaPossuiPagamento_ReturnsFalse()
        {
            // Arrange
            var mensagem = new MensagemPagamentoDto { PedidoId = Guid.NewGuid(), DataRecebimento = DateTime.Now, PagamentoId = Guid.NewGuid() };
            var pedido = new Pedido { Id = mensagem.PedidoId, Status = PagamentoHandler.STATUS_AGUARDANDO_PAGAMENTO, DataCriacao = DateTime.Now.AddDays(-1), PagamentoId = Guid.NewGuid() };
            _pedidoRepositoryMock.Setup(r => r.GetByIdAsync(mensagem.PedidoId)).ReturnsAsync(pedido);

            // Act and Assert
            Assert.False(await _pagamentoHandler.ProcessarAsync(mensagem));
        }
    }
}