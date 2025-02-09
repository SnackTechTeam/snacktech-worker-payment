using WorkerService.Data.Entities;

namespace WorkerService.Tests.Data.Entities
{
    public class PedidoTests
    {

        [Fact]
        public void Pedido_Id_SetAndGet()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pedido = new Pedido { Id = id };

            // Assert
            Assert.Equal(id, pedido.Id);
        }

        [Fact]
        public void Pedido_DataCriacao_SetAndGet()
        {
            // Arrange
            var dataCriacao = DateTime.Now;
            var pedido = new Pedido { DataCriacao = dataCriacao };

            // Assert
            Assert.Equal(dataCriacao, pedido.DataCriacao);
        }

        [Fact]
        public void Pedido_Status_SetAndGet()
        {
            // Arrange
            var status = 1;
            var pedido = new Pedido { Status = status };

            // Assert
            Assert.Equal(status, pedido.Status);
        }

        [Fact]
        public void Pedido_PagamentoId_SetAndGet()
        {
            // Arrange
            var pagamentoId = Guid.NewGuid();
            var pedido = new Pedido { PagamentoId = pagamentoId };

            // Assert
            Assert.Equal(pagamentoId, pedido.PagamentoId);
        }
    }
}