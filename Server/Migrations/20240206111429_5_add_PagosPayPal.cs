using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _5_add_PagosPayPal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Provincia",
                newName: "Provincias");

            migrationBuilder.RenameTable(
                name: "Municipio",
                newName: "Municipios");

            migrationBuilder.AddColumn<string>(
                name: "IdCliente",
                table: "Pedidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PagosPayPal",
                columns: table => new
                {
                    IdCliente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdPaypal = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPayPal", x => new { x.IdCliente, x.IdPaypal });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosPayPal");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Pedidos");

            migrationBuilder.RenameTable(
                name: "Provincias",
                newName: "Provincia");

            migrationBuilder.RenameTable(
                name: "Municipios",
                newName: "Municipio");
        }
    }
}
