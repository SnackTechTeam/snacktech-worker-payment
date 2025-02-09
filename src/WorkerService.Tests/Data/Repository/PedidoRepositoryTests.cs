using Moq;
using WorkerService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using WorkerService.Data;
using WorkerService.Data.Repository;

namespace WorkerService.Tests.Data.Repository
{
    public class PedidoRepositoryTests
    {
        private readonly Mock<PedidoContext> _contextMock;
        private readonly Mock<DbSet<Pedido>> _pedidoDbSetMock;
        private readonly PedidoRepository _repository;

        public PedidoRepositoryTests()
        {
            _pedidoDbSetMock = new Mock<DbSet<Pedido>>();
            var contextOptions = new DbContextOptions<PedidoContext>();
            _contextMock = new Mock<PedidoContext>(contextOptions);
            _contextMock.Setup(c => c.Pedidos).Returns(_pedidoDbSetMock.Object);
            _repository = new PedidoRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_PedidoExists_ReturnsPedido()
        {
            // Arrange
            var pedido = new Pedido { Id = Guid.NewGuid() };
            _pedidoDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(pedido);

            // Act
            var result = await _repository.GetByIdAsync(pedido.Id);

            // Assert
            Assert.Equal(pedido, result);
        }

        [Fact]
        public async Task GetByIdAsync_PedidoDoesNotExist_ReturnsNull()
        {
            // Arrange
            _pedidoDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((Pedido)null);

            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_PedidoExists_UpdatesPedido()
        {
            // Arrange
            var pedido = new Pedido { Id = Guid.NewGuid() };
            _pedidoDbSetMock.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync(pedido);
            _pedidoDbSetMock.Setup(m => m.Update(It.IsAny<Pedido>())).Verifiable();

            // Act
            await _repository.UpdateAsync(pedido);

            // Assert
            _pedidoDbSetMock.Verify(m => m.Update(It.IsAny<Pedido>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}