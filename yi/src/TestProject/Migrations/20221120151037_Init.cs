using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestProject.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "daily");

            migrationBuilder.CreateTable(
                name: "MarketOrderVms",
                schema: "daily",
                columns: table => new
                {
                    EpochSeconds = table.Column<long>(type: "bigint", nullable: false),
                    TimestampES = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StrategyName = table.Column<string>(type: "text", nullable: false),
                    Way = table.Column<string>(type: "text", nullable: false),
                    ExecNom = table.Column<double>(type: "double precision", nullable: false),
                    InstanceId = table.Column<string>(type: "text", nullable: false),
                    Counterparty = table.Column<string>(type: "text", nullable: false),
                    InstrumentType = table.Column<int>(type: "integer", nullable: false),
                    VenueCategory = table.Column<int>(type: "integer", nullable: false),
                    VenueId = table.Column<string>(type: "text", nullable: false),
                    VenueType = table.Column<int>(type: "integer", nullable: false),
                    TopLevelStrategyName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketOrderVms",
                schema: "daily");
        }
    }
}
