using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fitnes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimezoneOffsetMinutes",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimezoneOffsetMinutes",
                table: "Users");
        }
    }
}
