using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _2_addTables_Libros_Categorias_Pedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    IdCategoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreCategoria = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Libros",
                columns: table => new
                {
                    ISBN13 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    idCategoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Editorial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Autores = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenLibroBASE64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Edicion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dimensiones = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Idioma = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN10 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Resumen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroPaginas = table.Column<int>(type: "int", nullable: false),
                    Precio = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libros", x => x.ISBN13);
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    IdPedido = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FechaPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    listaItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecogerEnTienda = table.Column<bool>(type: "bit", nullable: false),
                    DireccionEnvioIdDireccion = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DireccionFacturacionIdDireccion = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Subtotal = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    GastosEnvio = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    Total = table.Column<decimal>(type: "DECIMAL(5,2)", nullable: false),
                    EstadoPedido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumItems = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedidos_Direcciones_DireccionEnvioIdDireccion",
                        column: x => x.DireccionEnvioIdDireccion,
                        principalTable: "Direcciones",
                        principalColumn: "IdDireccion");
                    table.ForeignKey(
                        name: "FK_Pedidos_Direcciones_DireccionFacturacionIdDireccion",
                        column: x => x.DireccionFacturacionIdDireccion,
                        principalTable: "Direcciones",
                        principalColumn: "IdDireccion");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionEnvioIdDireccion",
                table: "Pedidos",
                column: "DireccionEnvioIdDireccion");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionFacturacionIdDireccion",
                table: "Pedidos",
                column: "DireccionFacturacionIdDireccion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Libros");

            migrationBuilder.DropTable(
                name: "Pedidos");
        }
    }
}
