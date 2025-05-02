using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyect_Snake_West.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarCantidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cantidad",
                table: "t_proforma",
                newName: "Cantidad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cantidad",
                table: "t_proforma",
                newName: "cantidad");
        }
    }
}
