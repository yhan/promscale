using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Migrations
{
    /// <inheritdoc />
    public partial class AddId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "MarketOrderVms",
                schema: "daily",
                newName: "MarketOrderVms");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "MarketOrderVms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MarketOrderVms",
                table: "MarketOrderVms",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MarketOrderVms",
                table: "MarketOrderVms");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MarketOrderVms");

            migrationBuilder.EnsureSchema(
                name: "daily");

            migrationBuilder.RenameTable(
                name: "MarketOrderVms",
                newName: "MarketOrderVms",
                newSchema: "daily");
        }
    }
}
