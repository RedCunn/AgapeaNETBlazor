using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _8_moddify_PagosStripe_added_ChargeID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PagosStripe",
                table: "PagosStripe");

            migrationBuilder.AddColumn<string>(
                name: "ChargeId",
                table: "PagosStripe",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PagosStripe",
                table: "PagosStripe",
                columns: new[] { "IdCliente", "IdPedido", "ChargeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PagosStripe",
                table: "PagosStripe");

            migrationBuilder.DropColumn(
                name: "ChargeId",
                table: "PagosStripe");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PagosStripe",
                table: "PagosStripe",
                columns: new[] { "IdCliente", "IdPedido" });
        }
    }
}
