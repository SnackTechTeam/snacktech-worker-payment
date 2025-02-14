using WorkerService.Data.Entities;
using WorkerService.Data.Repository;
using WorkerService.DTOs;

namespace WorkerService.Handlers {
    public class PagamentoHandler : IPagamentoHandler
    {
        private readonly ILogger<PagamentoHandler> logger;
        private readonly IPedidoRepository pedidoRepository;
        public const int STATUS_AGUARDANDO_PAGAMENTO = 2;
        public const int STATUS_PAGAMENTO_REALIZADO = 3;

        public PagamentoHandler(ILogger<PagamentoHandler> logger, IPedidoRepository pedidoRepository)
        {
            this.logger = logger;
            this.pedidoRepository = pedidoRepository;
        }

        public async Task<bool> ProcessarAsync(MensagemPagamentoDto mensagem)
        {
            Pedido? pedido = await pedidoRepository.GetByIdAsync(mensagem.PedidoId);
            if(!OperacaoValida(mensagem, pedido)) return false;

            pedido!.Status = STATUS_PAGAMENTO_REALIZADO;
            pedido.PagamentoId = mensagem.PagamentoId;

            await pedidoRepository.UpdateAsync(pedido);

            return true;
        }

        private bool OperacaoValida(MensagemPagamentoDto mensagem, Pedido? pedido)
        {
            if (pedido == null)
            {
                logger.LogError("Pedido com ID {pedidoId} não encontrado.", mensagem.PedidoId);
                return false;
            }

            if (pedido.Status != STATUS_AGUARDANDO_PAGAMENTO)
            {
                logger.LogError("Pedido com ID {pedidoId} não está aguardando pagamento.", mensagem.PedidoId);
                return false;
            }

            if (pedido.PagamentoId != null && pedido.PagamentoId != Guid.Empty && pedido.PagamentoId != mensagem.PagamentoId)
            {
                logger.LogError("Pedido com ID {pedidoId} já possui um pagamento.", mensagem.PedidoId);
                return false;
            }

            if (mensagem.DataRecebimento < pedido.DataCriacao)
            {
                logger.LogError("Pedido com ID {pedidoId} foi criado depois do pagamento.", mensagem.PedidoId);
                return false;
            }

            return true;
        }
    }
}
