using System;
using Microsoft.EntityFrameworkCore.Migrations;
using fitnes.Domain.Constants.Persistence;
using fitnes.Domain.Models.Fitnes;
using fitnes.Infrastructure.Constants;

#nullable disable

namespace fitnes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "food_analysis_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    dish_name = table.Column<string>(type: "text", nullable: true),
                    confidence_score = table.Column<double>(type: "double precision", nullable: false),
                    nutrition = table.Column<Nutrition>(type: "jsonb", nullable: true),
                    serving_value = table.Column<double>(type: "double precision", nullable: true),
                    serving_unit = table.Column<string>(type: "text", nullable: true),
                    serving_description = table.Column<string>(type: "text", nullable: true),
                    analysis_summary = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_food_analysis_results", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "meal_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    consumed_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    source = table.Column<string>(type: "text", nullable: false),
                    analysis_result_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_meal_entries", x => x.id);
                    table.ForeignKey(
                        name: "FK_meal_entries_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_meal_entries_food_analysis_results_analysis_result_id",
                        column: x => x.analysis_result_id,
                        principalTable: "food_analysis_results",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_meal_entries_analysis_result_id",
                table: "meal_entries",
                column: "analysis_result_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_meal_entries_user_id_consumed_at",
                table: "meal_entries",
                columns: new[] { "user_id", "consumed_at" },
                descending: new[] { false, true });

            migrationBuilder.Sql($"""
                CREATE OR REPLACE VIEW "{ConfigurationConstants.MealEntryAveragesViewName}" AS
                SELECT
                    m."{MealEntryColumnConstants.Id}" AS "{MealEntryAveragesViewColumnConstants.Id}",
                    m."{MealEntryColumnConstants.UserId}" AS "{MealEntryAveragesViewColumnConstants.UserId}",
                    m."{MealEntryColumnConstants.ConsumedAt}" AS "{MealEntryAveragesViewColumnConstants.ConsumedAt}",
                    m."{MealEntryColumnConstants.Source}" AS "{MealEntryAveragesViewColumnConstants.Source}",
                    f."{FoodAnalysisResultColumnConstants.DishName}" AS "{MealEntryAveragesViewColumnConstants.DishName}",
                    f."{FoodAnalysisResultColumnConstants.ConfidenceScore}" AS "{MealEntryAveragesViewColumnConstants.ConfidenceScore}",
                    CASE WHEN f."{FoodAnalysisResultColumnConstants.Nutrition}" ? '{NutritionJsonConstants.CaloriesKcal}'
                        THEN (((f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.CaloriesKcal}'->>'{NutritionJsonConstants.Min}')::double precision + (f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.CaloriesKcal}'->>'{NutritionJsonConstants.Max}')::double precision) / 2)
                        ELSE NULL END AS "{MealEntryAveragesViewColumnConstants.AvgCalories}",
                    CASE WHEN f."{FoodAnalysisResultColumnConstants.Nutrition}" ? '{NutritionJsonConstants.ProteinG}'
                        THEN (((f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.ProteinG}'->>'{NutritionJsonConstants.Min}')::double precision + (f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.ProteinG}'->>'{NutritionJsonConstants.Max}')::double precision) / 2)
                        ELSE NULL END AS "{MealEntryAveragesViewColumnConstants.AvgProtein}",
                    CASE WHEN f."{FoodAnalysisResultColumnConstants.Nutrition}" ? '{NutritionJsonConstants.FatG}'
                        THEN (((f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.FatG}'->>'{NutritionJsonConstants.Min}')::double precision + (f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.FatG}'->>'{NutritionJsonConstants.Max}')::double precision) / 2)
                        ELSE NULL END AS "{MealEntryAveragesViewColumnConstants.AvgFat}",
                    CASE WHEN f."{FoodAnalysisResultColumnConstants.Nutrition}" ? '{NutritionJsonConstants.CarbsG}'
                        THEN (((f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.CarbsG}'->>'{NutritionJsonConstants.Min}')::double precision + (f."{FoodAnalysisResultColumnConstants.Nutrition}"->'{NutritionJsonConstants.CarbsG}'->>'{NutritionJsonConstants.Max}')::double precision) / 2)
                        ELSE NULL END AS "{MealEntryAveragesViewColumnConstants.AvgCarbs}"
                FROM "meal_entries" m
                LEFT JOIN "{ConfigurationConstants.FoodAnalysisResultTableName}" f ON f."{FoodAnalysisResultColumnConstants.Id}" = m."{MealEntryColumnConstants.AnalysisResultId}";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"""DROP VIEW IF EXISTS "{ConfigurationConstants.MealEntryAveragesViewName}";""");

            migrationBuilder.DropTable(
                name: "meal_entries");

            migrationBuilder.DropTable(
                name: "food_analysis_results");
        }
    }
}
