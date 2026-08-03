namespace fitnes.Infrastructure.Constants;

internal static class ConfigurationConstants
{
    public const string UserTableName = "Users";
    public const string MealLogTableName = "meal_log";
    public const string FoodAnalysisResultTableName = "food_analysis_results";
    public const string UserGoalTableName = "user_goals";
    public const string MealEntryAveragesViewName = "meal_entry_averages";
    public const string TimestamptzColumnType = "timestamptz";
    public const string JsonbColumnType = "jsonb";
    public const string NowDefaultSql = "now()";
    public const string GenRandomUuidDefaultSql = "gen_random_uuid()";
}
