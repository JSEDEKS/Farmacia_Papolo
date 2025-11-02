using Microsoft.EntityFrameworkCore;

namespace Farmacia_Paolo.Models
{
    public class FarmaciaContext : DbContext
    {
        public FarmaciaContext(DbContextOptions<FarmaciaContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }
    }
}
