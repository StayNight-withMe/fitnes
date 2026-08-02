using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTester.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false),
                Language = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.Sql(@"
                CREATE RULE ""Ignore_Duplicate_Users"" AS
                ON INSERT TO ""Users""
                WHERE EXISTS (
                    SELECT 1 FROM ""Users"" WHERE ""Id"" = NEW.""Id""
                )
                DO INSTEAD NOTHING;
            ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users");
    }
}
