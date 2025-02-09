
using WorkerService.DTOs;

namespace WorkerService.Handlers {
    public interface IPagamentoHandler
    {
        Task ProcessarAsync(MensagemPagamentoDto mensagem);
    }
}