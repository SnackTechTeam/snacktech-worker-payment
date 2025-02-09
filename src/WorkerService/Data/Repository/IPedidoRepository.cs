using WorkerService.Data.Entities;

namespace WorkerService.Data.Repository
{
    public interface IPedidoRepository
    {
        Task<Pedido?> GetByIdAsync(Guid pedidoId);
        Task UpdateAsync(Pedido pedido);
    }
}