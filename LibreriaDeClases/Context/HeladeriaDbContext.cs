using LibreriaDeClases.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaHeladeria.Data
{
    public class HeladeriaDbContext : DbContext
    {
        public HeladeriaDbContext(DbContextOptions<HeladeriaDbContext> options) : base(options)
        {
        }
        public DbSet<Helado> Helados { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Helado>()
                .HasOne(h => h.Categoria)
                .WithMany(n => n.Helados)
                .HasForeignKey(helado => helado.IdCategoria);

            modelBuilder.Entity<Helado>()
                .Property(h => h.Precio)
                .HasPrecision(18, 2); 
        }




    }
}
