using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerServiceCampaignAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 1,
                column: "createdDate",
                value: new DateTime(2023, 10, 14, 22, 40, 5, 143, DateTimeKind.Local).AddTicks(1038));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 2,
                column: "createdDate",
                value: new DateTime(2023, 10, 14, 22, 40, 5, 143, DateTimeKind.Local).AddTicks(1108));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 3,
                column: "createdDate",
                value: new DateTime(2023, 10, 14, 22, 40, 5, 143, DateTimeKind.Local).AddTicks(1114));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 1,
                column: "createdDate",
                value: new DateTime(2023, 10, 14, 21, 3, 28, 698, DateTimeKind.Local).AddTicks(7427));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 2,
                column: "createdDate",
                value: new DateTime(2023, 10, 14, 21, 3, 28, 698, DateTimeKind.Local).AddTicks(7491));

            migrationBuilder.UpdateData(
                table: "Agents",
                keyColumn: "Id",
                keyValue: 3,
                column: "createdDate",
                value: new DateTime(2023, 10, 14, 21, 3, 28, 698, DateTimeKind.Local).AddTicks(7493));
        }
    }
}
