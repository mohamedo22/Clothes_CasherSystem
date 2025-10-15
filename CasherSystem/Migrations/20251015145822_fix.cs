using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasherSystem.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "paidAmount",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "remainingAmount",
                table: "Sales",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paidAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "remainingAmount",
                table: "Sales");
        }
    }
}
