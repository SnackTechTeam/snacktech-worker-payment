using WorkerService.Data.Entities;
using WorkerService.Data.Repository;
using WorkerService.DTOs;

namespace WorkerService.Handlers {
    public class PedidoHandler : IPedidoHandler
    {
        private readonly ILogger<PedidoHandler> logger;
        private readonly IPedidoRepository pedidoRepository;

        public PedidoHandler(ILogger<PedidoHandler> logger, IPedidoRepository pedidoRepository)
        {
            this.logger = logger;
            this.pedidoRepository = pedidoRepository;
        }

        public async Task ProcessarPedidoAsync(MensagemPedidoDto mensagem)
        {
            Pedido? pedido = await pedidoRepository.GetByIdAsync(mensagem.PedidoId);

            if (pedido == null)
            {
                throw new InvalidOperationException($"Pedido com ID {mensagem.PedidoId} não encontrado.");
            }

            // TODO: trocar numero magico
            if (pedido.Status != 2)
            {
                throw new InvalidOperationException($"Pedido com ID {mensagem.PedidoId} não está aguardando pagamento.");
            }
            
            // TODO: trocar numero magico
            pedido.Status = 3;
            pedido.PagamentoId = mensagem.PagamentoId;

            await pedidoRepository.UpdateAsync(pedido);
        }
    }
}
