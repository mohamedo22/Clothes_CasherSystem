using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CasherSystem.Migrations
{
    /// <inheritdoc />
    public partial class removeSeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "syProducts",
                keyColumn: "Id",
                keyValue: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PhoneNumber", "Username" },
                values: new object[,]
                {
                    { 1, "01234567890", "admin" },
                    { 2, "01234567891", "seller1" },
                    { 3, "01234567892", "seller2" }
                });

            migrationBuilder.InsertData(
                table: "syProducts",
                columns: new[] { "Id", "Barcode", "Color", "CostPrice", "IsActive", "Name", "Quantity", "SellPrice", "Size", "counterOfSell" },
                values: new object[,]
                {
                    { 1, "TSH001", "أزرق", 15.00m, true, "قميص قطني", 50, 25.00m, "M", 0 },
                    { 2, "JEA001", "أزرق", 35.00m, true, "جينز", 30, 60.00m, "L", 0 },
                    { 3, "HOO001", "أسود", 40.00m, true, "هودي", 20, 75.00m, "XL", 0 },
                    { 4, "DRE001", "أحمر", 25.00m, true, "فستان صيفي", 15, 45.00m, "S", 0 },
                    { 5, "SNE001", "أبيض", 50.00m, true, "حذاء رياضي", 25, 90.00m, "42", 0 },
                    { 6, "SHI001", "أبيض", 30.00m, true, "قميص رسمي", 40, 55.00m, "L", 0 },
                    { 7, "PAN001", "رمادي", 45.00m, true, "بنطلون كاجوال", 35, 80.00m, "M", 0 },
                    { 8, "JAC001", "بني", 80.00m, true, "جاكيت شتوي", 12, 150.00m, "XL", 0 }
                });
        }
    }
}
