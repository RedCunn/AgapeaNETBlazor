using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _7_add_PagosStripe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdCliente",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PagosStripe",
                columns: table => new
                {
                    IdCliente = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdPedido = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosStripe", x => new { x.IdCliente, x.IdPedido });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosStripe");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Direcciones");
        }
    }
}
