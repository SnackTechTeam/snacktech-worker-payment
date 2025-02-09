
using WorkerService.DTOs;

namespace WorkerService.Handlers {
    public interface IPedidoHandler
    {
        Task ProcessarPedidoAsync(MensagemPedidoDto mensagem);
    }
}