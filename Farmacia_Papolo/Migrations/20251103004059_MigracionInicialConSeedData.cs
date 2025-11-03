using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Farmacia_Papolo.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicialConSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    ProveedorID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreProveedor = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    RNC = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.ProveedorID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RolID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreRol = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RolID);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StockMinimo = table.Column<int>(type: "INTEGER", nullable: false),
                    ProveedorID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ProductoID);
                    table.ForeignKey(
                        name: "FK_Productos_Proveedores_ProveedorID",
                        column: x => x.ProveedorID,
                        principalTable: "Proveedores",
                        principalColumn: "ProveedorID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreUsuario = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    RolID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.UsuarioID);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolID",
                        column: x => x.RolID,
                        principalTable: "Roles",
                        principalColumn: "RolID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lotes",
                columns: table => new
                {
                    LoteID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroLote = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CantidadActual = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaEntrada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProductoID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotes", x => x.LoteID);
                    table.ForeignKey(
                        name: "FK_Lotes_Productos_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Productos",
                        principalColumn: "ProductoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    MovimientoID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoMovimiento = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaMovimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    LoteID = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.MovimientoID);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Lotes_LoteID",
                        column: x => x.LoteID,
                        principalTable: "Lotes",
                        principalColumn: "LoteID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Usuarios_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuarios",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "ProveedorID", "NombreProveedor", "RNC", "Telefono" },
                values: new object[,]
                {
                    { 1, "Laboratorios MK", "101001001", "809-555-1111" },
                    { 2, "Farmax", "102002002", "809-555-2222" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RolID", "NombreRol" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Vendedor" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "ProductoID", "Categoria", "Codigo", "Nombre", "Precio", "ProveedorID", "StockMinimo" },
                values: new object[,]
                {
                    { 1, "Analgésico", "PROD-001", "Acetaminofén 500mg (Caja x20)", 150.00m, 1, 20 },
                    { 2, "Antibiótico", "PROD-002", "Amoxicilina 250mg (Suspensión)", 450.00m, 2, 10 },
                    { 3, "Vitaminas", "PROD-003", "Vitamina C 1000mg (Tubo x10)", 700.00m, 1, 15 }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "UsuarioID", "NombreCompleto", "NombreUsuario", "PasswordHash", "RolID" },
                values: new object[,]
                {
                    { 1, "Admin del Sistema", "admin", "admin123", 1 },
                    { 2, "Juan Perez", "vendedor1", "vendedor123", 2 }
                });

            migrationBuilder.InsertData(
                table: "Lotes",
                columns: new[] { "LoteID", "CantidadActual", "FechaEntrada", "FechaVencimiento", "NumeroLote", "ProductoID" },
                values: new object[,]
                {
                    { 1, 100, new DateTime(2025, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOTE-A100", 1 },
                    { 2, 50, new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2027, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOTE-A200", 1 },
                    { 3, 75, new DateTime(2025, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOTE-B500", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lotes_ProductoID",
                table: "Lotes",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_LoteID",
                table: "MovimientosInventario",
                column: "LoteID");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_UsuarioID",
                table: "MovimientosInventario",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ProveedorID",
                table: "Productos",
                column: "ProveedorID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolID",
                table: "Usuarios",
                column: "RolID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "Lotes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
