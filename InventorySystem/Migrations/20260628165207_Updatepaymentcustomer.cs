using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventorySystem.Migrations
{
    /// <inheritdoc />
    public partial class Updatepaymentcustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "CustomerPayments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c0d04cb0-3f0c-42ad-bcbe-c9ed98d18262", "AQAAAAIAAYagAAAAEC5XlGXsXNhKwqb+l1pnxDopirlcAmCHCaMQMJyBKmccFbPyXmGH/vFM0UO/Jv0WTQ==", "77bcc417-1f8a-4e98-88ba-c78a3aad007f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "CustomerPayments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d9a56794-4084-4710-aa82-8537732d5804", "AQAAAAIAAYagAAAAEGCOCnkNn4mvvg2tBxXNpPIfjsY/4j3ArZCDY3UFYmCNkBQjQV5VqMwvqxxGRG2eHw==", "1b45367a-f91e-4370-8ba0-65121b462e6f" });
        }
    }
}
