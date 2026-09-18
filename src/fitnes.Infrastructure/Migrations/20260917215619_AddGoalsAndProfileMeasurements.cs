using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalsAndProfileMeasurements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "HipsCm",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NeckCm",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WaistCm",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_goals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    target_weight = table.Column<double>(type: "double precision", nullable: false),
                    activity = table.Column<int>(type: "integer", nullable: false),
                    start_weight = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_goals", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_goals_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_goals_user_id_is_active",
                table: "user_goals",
                columns: new[] { "user_id", "is_active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_goals");

            migrationBuilder.DropColumn(
                name: "HipsCm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NeckCm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WaistCm",
                table: "Users");
        }
    }
}
