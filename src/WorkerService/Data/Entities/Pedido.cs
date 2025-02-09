using WorkerService.Enums;

namespace WorkerService.Data.Entities;

public class Pedido
{
    public Guid Id { get; set; }
    public DateTime DataCriacao { get; set; }
    public int Status { get; set; }
    public Guid? PagamentoId { get; set; }
}