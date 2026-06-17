using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabaseall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLedgers_Invoices_InvoiceId",
                table: "CustomerLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_StockEntries_StockEntryId",
                table: "StockLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Suppliers_SupplierId",
                table: "StockLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Warranties_InvoiceItems_InvoiceItemId",
                table: "Warranties");

            migrationBuilder.DropForeignKey(
                name: "FK_Warranties_Invoices_InvoiceId",
                table: "Warranties");

            migrationBuilder.DropForeignKey(
                name: "FK_Warranties_Products_ProductId",
                table: "Warranties");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyClaims_Invoices_InvoiceId",
                table: "WarrantyClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyClaims_Products_ProductId",
                table: "WarrantyClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyClaims_Warranties_WarrantyId",
                table: "WarrantyClaims");

            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "StockEntries");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_WarrantyClaims_InvoiceId",
                table: "WarrantyClaims");

            migrationBuilder.DropIndex(
                name: "IX_WarrantyClaims_ProductId",
                table: "WarrantyClaims");

            migrationBuilder.DropIndex(
                name: "IX_Warranties_InvoiceId",
                table: "Warranties");

            migrationBuilder.DropIndex(
                name: "IX_Warranties_InvoiceItemId",
                table: "Warranties");

            migrationBuilder.DropIndex(
                name: "IX_StockLedgers_StockEntryId",
                table: "StockLedgers");

            migrationBuilder.DropIndex(
                name: "IX_StockLedgers_SupplierId",
                table: "StockLedgers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerLedgers_InvoiceId",
                table: "CustomerLedgers");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "WarrantyClaims");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "WarrantyClaims");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Warranties");

            migrationBuilder.DropColumn(
                name: "InvoiceItemId",
                table: "Warranties");

            migrationBuilder.DropColumn(
                name: "BalanceQuantity",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "EntryDate",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "QuantityIn",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "QuantityOut",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "StockEntryId",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "CustomerLedgers");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "CustomerLedgers");

            migrationBuilder.RenameColumn(
                name: "WarrantyId",
                table: "WarrantyClaims",
                newName: "WarrantyItemId");

            migrationBuilder.RenameColumn(
                name: "ResolutionNote",
                table: "WarrantyClaims",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ClaimStatus",
                table: "WarrantyClaims",
                newName: "ClaimNo");

            migrationBuilder.RenameIndex(
                name: "IX_WarrantyClaims_WarrantyId",
                table: "WarrantyClaims",
                newName: "IX_WarrantyClaims_WarrantyItemId");

            migrationBuilder.RenameColumn(
                name: "WarrantyStatus",
                table: "Warranties",
                newName: "WarrantyNo");

            migrationBuilder.RenameColumn(
                name: "WarrantyStartDate",
                table: "Warranties",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "WarrantyEndDate",
                table: "Warranties",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Warranties",
                newName: "SalesInvoiceId");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Warranties",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_Warranties_ProductId",
                table: "Warranties",
                newName: "IX_Warranties_SalesInvoiceId");

            migrationBuilder.RenameColumn(
                name: "TotalCost",
                table: "StockLedgers",
                newName: "StockOut");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Purchases",
                newName: "WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_UserId",
                table: "Purchases",
                newName: "IX_Purchases_WarehouseId");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Customers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "CustomerLedgers",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "ActionTaken",
                table: "WarrantyClaims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningBalance",
                table: "Suppliers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BalanceQty",
                table: "StockLedgers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StockIn",
                table: "StockLedgers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "StockLedgers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "WarehouseId",
                table: "StockLedgers",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<decimal>(
                name: "GrandTotal",
                table: "Purchases",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Purchases",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "Purchases",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningBalance",
                table: "Customers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ReferenceId",
                table: "CustomerLedgers",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PurchaseItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseItems_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesInvoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesInvoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierLedgers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReferenceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierLedgers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    PurchaseId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPayments_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierPayments_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarrantyClaimId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarrantyHistory_WarrantyClaims_WarrantyClaimId",
                        column: x => x.WarrantyClaimId,
                        principalTable: "WarrantyClaims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WarrantyItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WarrantyId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    SerialNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    WarrantyMonths = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarrantyItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarrantyItems_Warranties_WarrantyId",
                        column: x => x.WarrantyId,
                        principalTable: "Warranties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    SalesInvoiceId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerPayments_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SalesItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalesInvoiceId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesItems_SalesInvoices_SalesInvoiceId",
                        column: x => x.SalesInvoiceId,
                        principalTable: "SalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "02b9dbcc-dd0d-4f2a-bae7-eee0f0915738", "AQAAAAIAAYagAAAAEByYn0wIsivlthCWvl68M8vHEgsKuI9x6Xe0qkR0W5F98O5qLgd4E/NJ2rD6bvEi/A==", "f63fc658-d351-4671-93ee-1295d1632212" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_WarehouseId",
                table: "StockLedgers",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_CustomerId",
                table: "CustomerPayments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_SalesInvoiceId",
                table: "CustomerPayments",
                column: "SalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_PurchaseId",
                table: "PurchaseItems",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoices_CustomerId",
                table: "SalesInvoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_ProductId",
                table: "SalesItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_SalesInvoiceId",
                table: "SalesItems",
                column: "SalesInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierLedgers_SupplierId",
                table: "SupplierLedgers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_PurchaseId",
                table: "SupplierPayments",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_SupplierId",
                table: "SupplierPayments",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyHistory_WarrantyClaimId",
                table: "WarrantyHistory",
                column: "WarrantyClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyItems_ProductId",
                table: "WarrantyItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyItems_WarrantyId",
                table: "WarrantyItems",
                column: "WarrantyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Warehouses_WarehouseId",
                table: "Purchases",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLedgers_Warehouses_WarehouseId",
                table: "StockLedgers",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warranties_SalesInvoices_SalesInvoiceId",
                table: "Warranties",
                column: "SalesInvoiceId",
                principalTable: "SalesInvoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyClaims_WarrantyItems_WarrantyItemId",
                table: "WarrantyClaims",
                column: "WarrantyItemId",
                principalTable: "WarrantyItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Warehouses_WarehouseId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLedgers_Warehouses_WarehouseId",
                table: "StockLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_Warranties_SalesInvoices_SalesInvoiceId",
                table: "Warranties");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyClaims_WarrantyItems_WarrantyItemId",
                table: "WarrantyClaims");

            migrationBuilder.DropTable(
                name: "CustomerPayments");

            migrationBuilder.DropTable(
                name: "PurchaseItems");

            migrationBuilder.DropTable(
                name: "SalesItems");

            migrationBuilder.DropTable(
                name: "SupplierLedgers");

            migrationBuilder.DropTable(
                name: "SupplierPayments");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "WarrantyHistory");

            migrationBuilder.DropTable(
                name: "WarrantyItems");

            migrationBuilder.DropTable(
                name: "SalesInvoices");

            migrationBuilder.DropIndex(
                name: "IX_StockLedgers_WarehouseId",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "ActionTaken",
                table: "WarrantyClaims");

            migrationBuilder.DropColumn(
                name: "OpeningBalance",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "BalanceQty",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "StockIn",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "StockLedgers");

            migrationBuilder.DropColumn(
                name: "GrandTotal",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "OpeningBalance",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "CustomerLedgers");

            migrationBuilder.RenameColumn(
                name: "WarrantyItemId",
                table: "WarrantyClaims",
                newName: "WarrantyId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "WarrantyClaims",
                newName: "ResolutionNote");

            migrationBuilder.RenameColumn(
                name: "ClaimNo",
                table: "WarrantyClaims",
                newName: "ClaimStatus");

            migrationBuilder.RenameIndex(
                name: "IX_WarrantyClaims_WarrantyItemId",
                table: "WarrantyClaims",
                newName: "IX_WarrantyClaims_WarrantyId");

            migrationBuilder.RenameColumn(
                name: "WarrantyNo",
                table: "Warranties",
                newName: "WarrantyStatus");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Warranties",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Warranties",
                newName: "WarrantyStartDate");

            migrationBuilder.RenameColumn(
                name: "SalesInvoiceId",
                table: "Warranties",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Warranties",
                newName: "WarrantyEndDate");

            migrationBuilder.RenameIndex(
                name: "IX_Warranties_SalesInvoiceId",
                table: "Warranties",
                newName: "IX_Warranties_ProductId");

            migrationBuilder.RenameColumn(
                name: "StockOut",
                table: "StockLedgers",
                newName: "TotalCost");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "Purchases",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_WarehouseId",
                table: "Purchases",
                newName: "IX_Purchases_UserId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customers",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CustomerLedgers",
                newName: "Remarks");

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "WarrantyClaims",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "WarrantyClaims",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "Warranties",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<long>(
                name: "InvoiceItemId",
                table: "Warranties",
                type: "bigint",
                nullable: false,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<int>(
                name: "BalanceQuantity",
                table: "StockLedgers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EntryDate",
                table: "StockLedgers",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "QuantityIn",
                table: "StockLedgers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantityOut",
                table: "StockLedgers",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<long>(
                name: "SupplierId",
                table: "StockLedgers",
                type: "bigint",
                nullable: true,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InvoiceId",
                table: "CustomerLedgers",
                type: "bigint",
                nullable: true,
                defaultValueSql: "0");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "CustomerLedgers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    UserId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalProfit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPurchaseCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSellingAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalTaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    PurchaseId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    UserId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    BatchNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransportCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockEntries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockEntries_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockEntries_Purchases_PurchaseId",
                        column: x => x.PurchaseId,
                        principalTable: "Purchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockEntries_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "0"),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "773afceb-c371-4bd7-90fe-427452164355", "AQAAAAIAAYagAAAAEAf39JTjE8Lnfc141bGRf4pbrFES3axp4jkBr3VNgF6ecpaPJU2xNwEvEItHqDpLQw==", "36005595-983f-4ac8-9958-d1a46d571e6f" });

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_InvoiceId",
                table: "WarrantyClaims",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyClaims_ProductId",
                table: "WarrantyClaims",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_InvoiceId",
                table: "Warranties",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Warranties_InvoiceItemId",
                table: "Warranties",
                column: "InvoiceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_StockEntryId",
                table: "StockLedgers",
                column: "StockEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLedgers_SupplierId",
                table: "StockLedgers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLedgers_InvoiceId",
                table: "CustomerLedgers",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_ProductId",
                table: "InvoiceItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_UserId",
                table: "Invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_ProductId",
                table: "StockEntries",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_PurchaseId",
                table: "StockEntries",
                column: "PurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_SupplierId",
                table: "StockEntries",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockEntries_UserId",
                table: "StockEntries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLedgers_Invoices_InvoiceId",
                table: "CustomerLedgers",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_UserId",
                table: "Purchases",
                column: "UserId",
                principalTable: "AspNetUsers",
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
                name: "FK_StockLedgers_Suppliers_SupplierId",
                table: "StockLedgers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warranties_InvoiceItems_InvoiceItemId",
                table: "Warranties",
                column: "InvoiceItemId",
                principalTable: "InvoiceItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warranties_Invoices_InvoiceId",
                table: "Warranties",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warranties_Products_ProductId",
                table: "Warranties",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyClaims_Invoices_InvoiceId",
                table: "WarrantyClaims",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyClaims_Products_ProductId",
                table: "WarrantyClaims",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyClaims_Warranties_WarrantyId",
                table: "WarrantyClaims",
                column: "WarrantyId",
                principalTable: "Warranties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
