using WorkerService.Data.Entities;
using WorkerService.Data.Repository;
using WorkerService.DTOs;

namespace WorkerService.Handlers {
    public class PagamentoHandler : IPagamentoHandler
    {
        private readonly IPedidoRepository pedidoRepository;
        public const int STATUS_AGUARDANDO_PAGAMENTO = 2;
        public const int STATUS_PAGAMENTO_REALIZADO = 3;

        public PagamentoHandler(IPedidoRepository pedidoRepository)
        {
            this.pedidoRepository = pedidoRepository;
        }

        public async Task ProcessarAsync(MensagemPagamentoDto mensagem)
        {
            Pedido? pedido = await pedidoRepository.GetByIdAsync(mensagem.PedidoId);
            ValidarPreCondicoes(mensagem, pedido);

            pedido!.Status = STATUS_PAGAMENTO_REALIZADO;
            pedido.PagamentoId = mensagem.PagamentoId;

            await pedidoRepository.UpdateAsync(pedido);
        }

        private static void ValidarPreCondicoes(MensagemPagamentoDto mensagem, Pedido? pedido)
        {
            if (pedido == null)
            {
                throw new InvalidOperationException($"Pedido com ID {mensagem.PedidoId} não encontrado.");
            }

            if (pedido.Status != STATUS_AGUARDANDO_PAGAMENTO)
            {
                throw new InvalidOperationException($"Pedido com ID {mensagem.PedidoId} não está aguardando pagamento.");
            }

            if (pedido.PagamentoId != null && pedido.PagamentoId != mensagem.PagamentoId)
            {
                throw new InvalidOperationException($"Pedido com ID {mensagem.PedidoId} já possui um pagamento.");
            }

            if (mensagem.DataRecebimento < pedido.DataCriacao)
            {
                throw new InvalidOperationException($"Pedido com ID {mensagem.PedidoId} foi criado depois do pagamento.");
            }
        }
    }
}
