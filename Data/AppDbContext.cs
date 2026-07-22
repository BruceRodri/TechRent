using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechRent.Models;

namespace TechRent.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<EstadoReserva> EstadosReserva { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<DetalleReserva> DetalleReservas { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<OrdenAlquiler> OrdenesAlquiler { get; set; }
        public DbSet<DetalleOrdenAlquiler> DetallesOrdenAlquiler { get; set; }
        public DbSet<TransaccionPago> TransaccionesPago { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>().HasQueryFilter(c => c.Activo);
            modelBuilder.Entity<Marca>().HasQueryFilter(m => m.Activo);
            modelBuilder.Entity<Equipo>().HasQueryFilter(e => e.Activo);
            modelBuilder.Entity<Cliente>().HasQueryFilter(c => c.Activo);
            modelBuilder.Entity<EstadoReserva>().HasQueryFilter(e => e.Activo);
            modelBuilder.Entity<Reserva>().HasQueryFilter(r => r.Activo);
            modelBuilder.Entity<Pago>().HasQueryFilter(p => p.Activo);
            modelBuilder.Entity<Usuario>().HasQueryFilter(u => u.Activo);
            modelBuilder.Entity<DetalleReserva>().HasQueryFilter(d => d.Activo);

            modelBuilder.Entity<Equipo>()
                .HasOne(e => e.Categoria)
                .WithMany(c => c.Equipos)
                .HasForeignKey(e => e.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipo>()
                .HasOne(e => e.Marca)
                .WithMany(m => m.Equipos)
                .HasForeignKey(e => e.MarcaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.Cliente)
                .WithMany(c => c.Reservas)
                .HasForeignKey(r => r.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reserva>()
                .HasOne(r => r.EstadoReserva)
                .WithMany(e => e.Reservas)
                .HasForeignKey(r => r.EstadoReservaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleReserva>()
                .HasOne(d => d.Reserva)
                .WithMany(r => r.DetalleReservas)
                .HasForeignKey(d => d.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleReserva>()
                .HasOne(d => d.Equipo)
                .WithMany(e => e.DetalleReservas)
                .HasForeignKey(d => d.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Reserva)
                .WithMany(r => r.Pagos)
                .HasForeignKey(p => p.ReservaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Equipo>()
                .Property(e => e.PrecioPorDia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Reserva>()
                .Property(r => r.MontoTotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleReserva>()
                .Property(d => d.PrecioUnitarioPorDia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleReserva>()
                .Property(d => d.Subtotal)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pago>()
                .Property(p => p.Monto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrdenAlquiler>()
                .Property(o => o.Total)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleOrdenAlquiler>()
                .Property(d => d.PrecioPorDia)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DetalleOrdenAlquiler>()
                .Property(d => d.Subtotal)
                .HasPrecision(18, 2);
        }
    }
}
