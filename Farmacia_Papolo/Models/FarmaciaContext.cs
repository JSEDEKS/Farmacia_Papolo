using Microsoft.EntityFrameworkCore;

namespace Farmacia_Paolo.Models
{
    public class FarmaciaContext : DbContext
    {
        public FarmaciaContext(DbContextOptions<FarmaciaContext> options)
            : base(options)
        {
        }

        // --- Tablas Principales ---
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }

        // --- Tablas de Soporte (Catálogos) ---
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        // --- Configuración Adicional---
        // REEMPLAZA TU MÉTODO OnModelCreating CON ESTE:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CONFIGURACIÓN DE MODELOS ---

            // Configura el tipo de columna para Precio
            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18, 2)");

            // Configura las reglas de borrado (para evitar errores)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Proveedor)
                .WithMany(prov => prov.Productos)
                .HasForeignKey(p => p.ProveedorID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lote>()
                .HasOne(l => l.Producto)
                .WithMany(p => p.Lotes)
                .HasForeignKey(l => l.ProductoID)
                .OnDelete(DeleteBehavior.Restrict);

            // --- DATOS DE PRUEBA (SEED DATA) ---
            // La inserción de datos debe seguir el orden de dependencias.

            // 1. Roles (Sin dependencias)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolID = 1, NombreRol = "Administrador" },
                new Rol { RolID = 2, NombreRol = "Vendedor" }
            );

            // 2. Proveedores (Sin dependencias)
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor { ProveedorID = 1, NombreProveedor = "Laboratorios MK", RNC = "101001001", Telefono = "809-555-1111" },
                new Proveedor { ProveedorID = 2, NombreProveedor = "Farmax", RNC = "102002002", Telefono = "809-555-2222" }
            );

            // 3. Usuarios (Depende de Roles)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    UsuarioID = 1,
                    NombreUsuario = "admin",
                    NombreCompleto = "Admin del Sistema",
                    // ¡IMPORTANTE! Esto NO es una contraseña segura. 
                    // Es solo un placeholder para la demo.
                    // En una app real, este hash se genera, no se escribe.
                    PasswordHash = "admin123",
                    RolID = 1 // Rol "Administrador"
                },
                new Usuario
                {
                    UsuarioID = 2,
                    NombreUsuario = "vendedor1",
                    NombreCompleto = "Juan Perez",
                    PasswordHash = "vendedor123",
                    RolID = 2 // Rol "Vendedor"
                }
            );

            // 4. Productos (Depende de Proveedores)
            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    ProductoID = 1,
                    Nombre = "Acetaminofén 500mg (Caja x20)",
                    Codigo = "PROD-001",
                    Categoria = "Analgésico",
                    Precio = 150.00m,
                    StockMinimo = 20,
                    ProveedorID = 1 // Laboratorios MK
                },
                new Producto
                {
                    ProductoID = 2,
                    Nombre = "Amoxicilina 250mg (Suspensión)",
                    Codigo = "PROD-002",
                    Categoria = "Antibiótico",
                    Precio = 450.00m,
                    StockMinimo = 10,
                    ProveedorID = 2 // Farmax
                },
                new Producto
                {
                    ProductoID = 3,
                    Nombre = "Vitamina C 1000mg (Tubo x10)",
                    Codigo = "PROD-003",
                    Categoria = "Vitaminas",
                    Precio = 700.00m,
                    StockMinimo = 15,
                    ProveedorID = 1 // Laboratorios MK
                }
            );

            // 5. Lotes (Depende de Productos)
            // 5. Lotes (Depende de Productos)
            modelBuilder.Entity<Lote>().HasData(
                new Lote
                {
                    LoteID = 1,
                    ProductoID = 1, // Acetaminofén
                    NumeroLote = "LOTE-A100",
                    CantidadActual = 100,
                    // MODIFICADO: Fecha estática
                    FechaEntrada = new DateTime(2025, 10, 01),
                    FechaVencimiento = new DateTime(2026, 12, 31)
                },
                new Lote
                {
                    LoteID = 2,
                    ProductoID = 1, // Acetaminofén
                    NumeroLote = "LOTE-A200",
                    CantidadActual = 50,
                    // MODIFICADO: Fecha estática
                    FechaEntrada = new DateTime(2025, 10, 15),
                    FechaVencimiento = new DateTime(2027, 05, 30)
                },
                new Lote
                {
                    LoteID = 3,
                    ProductoID = 2, // Amoxicilina
                    NumeroLote = "LOTE-B500",
                    CantidadActual = 75,
                    // MODIFICADO: Fecha estática
                    FechaEntrada = new DateTime(2025, 10, 20),
                    FechaVencimiento = new DateTime(2026, 08, 15)
                }
            );
        }
    }
}