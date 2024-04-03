using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agapea_Blazor.Server.Migrations
{
    /// <inheritdoc />
    public partial class _4_add_Provincias_Municipios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Municipio",
                columns: table => new
                {
                    CPRO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CMUM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CUN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DMUN50 = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    CCOM = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CPRO = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PRO = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Municipio");

            migrationBuilder.DropTable(
                name: "Provincia");
        }
    }
}
