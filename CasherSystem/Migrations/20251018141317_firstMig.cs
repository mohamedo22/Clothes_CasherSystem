using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CasherSystem.Migrations
{
    /// <inheritdoc />
    public partial class firstMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Supplier_Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Supplier_PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "syProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Barcode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Size = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Color = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    CostPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    SellPrice = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    counterOfSell = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_syProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    NetTotal = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    paidAmount = table.Column<int>(type: "INTEGER", nullable: true),
                    remainingAmount = table.Column<int>(type: "INTEGER", nullable: true),
                    SecretCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Returns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SaleId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TotalRefund = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    UserInfoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Returns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Returns_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Returns_Users_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SaledProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    saledQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    SaleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaledProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaledProducts_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaledProducts_syProducts_ProductId",
                        column: x => x.ProductId,
                        principalTable: "syProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaledProducts_syProducts_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "syProducts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProductReturn",
                columns: table => new
                {
                    ReturnsId = table.Column<int>(type: "INTEGER", nullable: false),
                    productsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReturn", x => new { x.ReturnsId, x.productsId });
                    table.ForeignKey(
                        name: "FK_ProductReturn_Returns_ReturnsId",
                        column: x => x.ReturnsId,
                        principalTable: "Returns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReturn_syProducts_productsId",
                        column: x => x.productsId,
                        principalTable: "syProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductReturn_productsId",
                table: "ProductReturn",
                column: "productsId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_SaleId",
                table: "Returns",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Returns_UserInfoId",
                table: "Returns",
                column: "UserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_SaledProducts_ProductId",
                table: "SaledProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaledProducts_ProductId1",
                table: "SaledProducts",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_SaledProducts_SaleId",
                table: "SaledProducts",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UserId",
                table: "Sales",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductReturn");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "SaledProducts");

            migrationBuilder.DropTable(
                name: "Returns");

            migrationBuilder.DropTable(
                name: "syProducts");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
