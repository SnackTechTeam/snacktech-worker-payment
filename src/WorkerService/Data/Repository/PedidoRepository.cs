using WorkerService.Data.Entities;

namespace WorkerService.Data.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidoContext context;

        public PedidoRepository(PedidoContext context)
        {
            this.context = context;
        }

        public async Task<Pedido?> GetByIdAsync(Guid pedidoId)
        {
            return await context.Pedido.FindAsync(pedidoId);
        }

        public async Task UpdateAsync(Pedido pedido)
        {
            context.Pedido.Update(pedido);
            await context.SaveChangesAsync();
        }
    }
}