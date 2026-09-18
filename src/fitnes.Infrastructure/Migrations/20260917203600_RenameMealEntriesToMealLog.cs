using Microsoft.EntityFrameworkCore.Migrations;
using fitnes.Domain.Constants.Persistence;
using fitnes.Infrastructure.Constants;

#nullable disable

namespace fitnes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameMealEntriesToMealLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meal_entries_Users_user_id",
                table: "meal_entries");

            migrationBuilder.DropForeignKey(
                name: "FK_meal_entries_food_analysis_results_analysis_result_id",
                table: "meal_entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_meal_entries",
                table: "meal_entries");

            migrationBuilder.RenameTable(
                name: "meal_entries",
                newName: "meal_log");

            migrationBuilder.RenameIndex(
                name: "IX_meal_entries_user_id_consumed_at",
                table: "meal_log",
                newName: "IX_meal_log_user_id_consumed_at");

            migrationBuilder.RenameIndex(
                name: "IX_meal_entries_analysis_result_id",
                table: "meal_log",
                newName: "IX_meal_log_analysis_result_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_meal_log",
                table: "meal_log",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_meal_log_Users_user_id",
                table: "meal_log",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_meal_log_food_analysis_results_analysis_result_id",
                table: "meal_log",
                column: "analysis_result_id",
                principalTable: "food_analysis_results",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql($"""DROP VIEW IF EXISTS "{ConfigurationConstants.MealEntryAveragesViewName}";""");

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
                FROM "{ConfigurationConstants.MealLogTableName}" m
                LEFT JOIN "{ConfigurationConstants.FoodAnalysisResultTableName}" f ON f."{FoodAnalysisResultColumnConstants.Id}" = m."{MealEntryColumnConstants.AnalysisResultId}";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meal_log_Users_user_id",
                table: "meal_log");

            migrationBuilder.DropForeignKey(
                name: "FK_meal_log_food_analysis_results_analysis_result_id",
                table: "meal_log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_meal_log",
                table: "meal_log");

            migrationBuilder.RenameTable(
                name: "meal_log",
                newName: "meal_entries");

            migrationBuilder.RenameIndex(
                name: "IX_meal_log_user_id_consumed_at",
                table: "meal_entries",
                newName: "IX_meal_entries_user_id_consumed_at");

            migrationBuilder.RenameIndex(
                name: "IX_meal_log_analysis_result_id",
                table: "meal_entries",
                newName: "IX_meal_entries_analysis_result_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_meal_entries",
                table: "meal_entries",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_meal_entries_Users_user_id",
                table: "meal_entries",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_meal_entries_food_analysis_results_analysis_result_id",
                table: "meal_entries",
                column: "analysis_result_id",
                principalTable: "food_analysis_results",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.Sql($"""DROP VIEW IF EXISTS "{ConfigurationConstants.MealEntryAveragesViewName}";""");

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
    }
}
