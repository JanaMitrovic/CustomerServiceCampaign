using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServiceCampaignAPI.Migrations
{
    /// <inheritdoc />
    public partial class PurchasesModelChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Discount",
                table: "Purchases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Purchases");
        }
    }
}
