namespace WorkerService.DTOs;
public class MensagemPedidoDto
{
    public Guid PedidoId { get; set; } = default!;
    public Guid PagamentoId { get; set; } = default!;
    public DateTime DataRecebimento { get; set; } = default!;
    public string NomePlataforma { get; set; } = default!;
}