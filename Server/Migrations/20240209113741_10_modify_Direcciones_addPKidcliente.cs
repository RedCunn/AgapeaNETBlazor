using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _10_modify_Direcciones_addPKidcliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionEnvioIdDireccion",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionFacturacionIdDireccion",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_DireccionEnvioIdDireccion",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_DireccionFacturacionIdDireccion",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Direcciones",
                table: "Direcciones");

            migrationBuilder.AddColumn<string>(
                name: "DireccionEnvioIdCliente",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DireccionFacturacionIdCliente",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdCliente",
                table: "Direcciones",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Direcciones",
                table: "Direcciones",
                columns: new[] { "IdDireccion", "IdCliente" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionEnvioIdDireccion_DireccionEnvioIdCliente",
                table: "Pedidos",
                columns: new[] { "DireccionEnvioIdDireccion", "DireccionEnvioIdCliente" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionFacturacionIdDireccion_DireccionFacturacionIdCliente",
                table: "Pedidos",
                columns: new[] { "DireccionFacturacionIdDireccion", "DireccionFacturacionIdCliente" });

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionEnvioIdDireccion_DireccionEnvioIdCliente",
                table: "Pedidos",
                columns: new[] { "DireccionEnvioIdDireccion", "DireccionEnvioIdCliente" },
                principalTable: "Direcciones",
                principalColumns: new[] { "IdDireccion", "IdCliente" });

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionFacturacionIdDireccion_DireccionFacturacionIdCliente",
                table: "Pedidos",
                columns: new[] { "DireccionFacturacionIdDireccion", "DireccionFacturacionIdCliente" },
                principalTable: "Direcciones",
                principalColumns: new[] { "IdDireccion", "IdCliente" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionEnvioIdDireccion_DireccionEnvioIdCliente",
                table: "Pedidos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionFacturacionIdDireccion_DireccionFacturacionIdCliente",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_DireccionEnvioIdDireccion_DireccionEnvioIdCliente",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_DireccionFacturacionIdDireccion_DireccionFacturacionIdCliente",
                table: "Pedidos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Direcciones",
                table: "Direcciones");

            migrationBuilder.DropColumn(
                name: "DireccionEnvioIdCliente",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "DireccionFacturacionIdCliente",
                table: "Pedidos");

            migrationBuilder.AlterColumn<string>(
                name: "IdCliente",
                table: "Direcciones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Direcciones",
                table: "Direcciones",
                column: "IdDireccion");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionEnvioIdDireccion",
                table: "Pedidos",
                column: "DireccionEnvioIdDireccion");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_DireccionFacturacionIdDireccion",
                table: "Pedidos",
                column: "DireccionFacturacionIdDireccion");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionEnvioIdDireccion",
                table: "Pedidos",
                column: "DireccionEnvioIdDireccion",
                principalTable: "Direcciones",
                principalColumn: "IdDireccion");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Direcciones_DireccionFacturacionIdDireccion",
                table: "Pedidos",
                column: "DireccionFacturacionIdDireccion",
                principalTable: "Direcciones",
                principalColumn: "IdDireccion");
        }
    }
}
