using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ST10438307_GLMS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCostUSD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostUSD",
                table: "ServiceRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostUSD",
                table: "ServiceRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
