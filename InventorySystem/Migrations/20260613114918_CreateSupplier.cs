using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class CreateSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockEntries_Supplier_SupplierId",
                table: "StockEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Supplier_SupplierId",
                table: "StockLedgers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Supplier",
                table: "Supplier");

            migrationBuilder.RenameTable(
                name: "Supplier",
                newName: "Suppliers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c7a29edf-0160-40b9-946f-181d8c149da5", "AQAAAAIAAYagAAAAEISmBDsYkYHuWB1i2DyjXX22KEUg8vvdezUqSj61XFx5fUrATJhjMnZSt2pY79O/7Q==", "4859c0b8-3a78-424c-8f90-52fb94b9b3c4" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockEntries_Suppliers_SupplierId",
                table: "StockEntries",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgers_Suppliers_SupplierId",
                table: "StockLedgers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockEntries_Suppliers_SupplierId",
                table: "StockEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Suppliers_SupplierId",
                table: "StockLedgers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "Supplier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Supplier",
                table: "Supplier",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b39c22d6-cb31-4233-b3c5-132e617b43d8", "AQAAAAIAAYagAAAAECsqAkfxEVHnJT8lxL86xvSDMplJSq/w7LHvHX3gRGiB0n9QZZFoeHgni74rDyycWg==", "2e4eddf5-4faf-42ee-8cef-731cec9674b4" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockEntries_Supplier_SupplierId",
                table: "StockEntries",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgers_Supplier_SupplierId",
                table: "StockLedgers",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
