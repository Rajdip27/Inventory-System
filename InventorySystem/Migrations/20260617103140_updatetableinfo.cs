using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class updatetableinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "SupplierLedgers",
                newName: "OpeningBalance");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "CustomerLedgers",
                newName: "OpeningBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "ClosingBalance",
                table: "SupplierLedgers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ClosingBalance",
                table: "CustomerLedgers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d9a56794-4084-4710-aa82-8537732d5804", "AQAAAAIAAYagAAAAEGCOCnkNn4mvvg2tBxXNpPIfjsY/4j3ArZCDY3UFYmCNkBQjQV5VqMwvqxxGRG2eHw==", "1b45367a-f91e-4370-8ba0-65121b462e6f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingBalance",
                table: "SupplierLedgers");

            migrationBuilder.DropColumn(
                name: "ClosingBalance",
                table: "CustomerLedgers");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance",
                table: "SupplierLedgers",
                newName: "Balance");

            migrationBuilder.RenameColumn(
                name: "OpeningBalance",
                table: "CustomerLedgers",
                newName: "Balance");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "02b9dbcc-dd0d-4f2a-bae7-eee0f0915738", "AQAAAAIAAYagAAAAEByYn0wIsivlthCWvl68M8vHEgsKuI9x6Xe0qkR0W5F98O5qLgd4E/NJ2rD6bvEi/A==", "f63fc658-d351-4671-93ee-1295d1632212" });
        }
    }
}
