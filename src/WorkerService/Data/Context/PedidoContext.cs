using Microsoft.EntityFrameworkCore;
using WorkerService.Data.Entities;

namespace WorkerService.Data
{
    public class PedidoContext : DbContext
    {
        // Constructor to inject options and configure the context
        public PedidoContext(DbContextOptions<PedidoContext> options)
            : base(options)
        {
        }

        // Define a DbSet for the Pedido entity
        public virtual DbSet<Pedido> Pedido { get; set; }
    }
}