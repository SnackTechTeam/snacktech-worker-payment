
using WorkerService.DTOs;

namespace WorkerService.Handlers {
    public interface IPagamentoHandler
    {
        Task<bool> ProcessarAsync(MensagemPagamentoDto mensagem);
    }
}