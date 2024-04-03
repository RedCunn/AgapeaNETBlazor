using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _6_modify_PagosPayPal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PagosPayPal",
                table: "PagosPayPal");

            migrationBuilder.RenameColumn(
                name: "IdPaypal",
                table: "PagosPayPal",
                newName: "IdPagoPayPal");

            migrationBuilder.AddColumn<string>(
                name: "IdPedido",
                table: "PagosPayPal",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PagosPayPal",
                table: "PagosPayPal",
                columns: new[] { "IdCliente", "IdPedido", "IdPagoPayPal" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PagosPayPal",
                table: "PagosPayPal");

            migrationBuilder.DropColumn(
                name: "IdPedido",
                table: "PagosPayPal");

            migrationBuilder.RenameColumn(
                name: "IdPagoPayPal",
                table: "PagosPayPal",
                newName: "IdPaypal");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PagosPayPal",
                table: "PagosPayPal",
                columns: new[] { "IdCliente", "IdPaypal" });
        }
    }
}
