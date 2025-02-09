using Microsoft.EntityFrameworkCore;
using WorkerService.Data;
using WorkerService.Data.Entities;

namespace WorkerService.Tests.Data.Context
{
    public class PedidoContextTests
    {
        [Fact]
        public async Task PedidoContext_CreateDbContext_OptionsAreConfigured()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PedidoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            using var context = new PedidoContext(options);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public async Task PedidoContext_Pedidos_DbSetIsConfigured()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PedidoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            using var context = new PedidoContext(options);

            // Assert
            Assert.NotNull(context.Pedidos);
        }

        [Fact]
        public async Task PedidoContext_SaveChangesAsync_ChangesAreSaved()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<PedidoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new PedidoContext(options);

            var pedido = new Pedido { Id = Guid.NewGuid() };

            // Act
            context.Pedidos.Add(pedido);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(1, context.Pedidos.Count());
        }
    }
}