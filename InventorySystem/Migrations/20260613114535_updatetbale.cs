using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetbale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Products_ProductId1",
                table: "StockLedgers");

            migrationBuilder.RenameColumn(
                name: "ProductId1",
                table: "StockLedgers",
                newName: "SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_StockLedgers_ProductId1",
                table: "StockLedgers",
                newName: "IX_StockLedgers_SupplierId");

            migrationBuilder.AlterColumn<long>(
                name: "ReferenceId",
                table: "StockLedgers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "ProductId",
                table: "StockLedgers",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EntryDate",
                table: "StockLedgers",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "StockLedgers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StockEntryId",
                table: "StockLedgers",
                type: "bigint",
                nullable: true,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "StockLedgers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitCost",
                table: "StockLedgers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BatchNo",
                table: "StockEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "StockEntries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "StockEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "StockEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "StockEntries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "SupplierId",
                table: "StockEntries",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "StockEntries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransportCost",
                table: "StockEntries",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TradeLicense = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b39c22d6-cb31-4233-b3c5-132e617b43d8", "AQAAAAIAAYagAAAAECsqAkfxEVHnJT8lxL86xvSDMplJSq/w7LHvHX3gRGiB0n9QZZFoeHgni74rDyycWg==", "2e4eddf5-4faf-42ee-8cef-731cec9674b4" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_ProductId",
                table: "StockLedgers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_StockEntryId",
                table: "StockLedgers",
                column: "StockEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_SupplierId",
                table: "StockEntries",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockEntries_Supplier_SupplierId",
                table: "StockEntries",
                column: "SupplierId",
                principalTable: "Supplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgers_Products_ProductId",
                table: "StockLedgers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgers_StockEntries_StockEntryId",
                table: "StockLedgers",
                column: "StockEntryId",
                principalTable: "StockEntries",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockEntries_Supplier_SupplierId",
                table: "StockEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Products_ProductId",
                table: "StockLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_StockEntries_StockEntryId",
                table: "StockLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Supplier_SupplierId",
                table: "StockLedgers");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropIndex(
                name: "IX_StockLedgers_ProductId",
                table: "StockLedgers");

            migrationBuilder.DropIndex(
                name: "IX_StockLedgers_StockEntryId",
                table: "StockLedgers");

            migrationBuilder.DropIndex(
                name: "IX_StockEntries_SupplierId",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "StockEntryId",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "UnitCost",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "BatchNo",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "StockEntries");

            migrationBuilder.DropColumn(
                name: "TransportCost",
                table: "StockEntries");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "StockLedgers",
                newName: "ProductId1");

            migrationBuilder.RenameIndex(
                name: "IX_StockLedgers_SupplierId",
                table: "StockLedgers",
                newName: "IX_StockLedgers_ProductId1");

            migrationBuilder.AlterColumn<int>(
                name: "ReferenceId",
                table: "StockLedgers",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "StockLedgers",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValueSql: "0");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ddde673-cdf0-4bec-b0e8-715c6a7af341", "AQAAAAIAAYagAAAAED8fCTrkBcCoMPL7qDeH5i7LVWZcMdrJ6W3BVd/64j1fnjBmxuyNZw9+QmhL/Bs5Yw==", "f685e11b-acf5-46b8-bc58-187eb01c9c59" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgers_Products_ProductId1",
                table: "StockLedgers",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
