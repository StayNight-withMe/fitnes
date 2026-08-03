using fitnes.Domain.Constants.Persistence;
using fitnes.Domain.Models.Fitnes;
using fitnes.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fitnes.Infrastructure.Persistence.Configurations;

public static class FoodAnalysisResultConfiguration
{
    public static void ConfigureFoodAnalysisResult(this EntityTypeBuilder<FoodAnalysisResult> builder)
    {
        builder.ToTable(ConfigurationConstants.FoodAnalysisResultTableName);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName(FoodAnalysisResultColumnConstants.Id).HasDefaultValueSql(ConfigurationConstants.GenRandomUuidDefaultSql);
        builder.Property(e => e.Dish_Name).HasColumnName(FoodAnalysisResultColumnConstants.DishName);
        builder.Property(e => e.Confidence_Score).HasColumnName(FoodAnalysisResultColumnConstants.ConfidenceScore).IsRequired();
        builder.Property(e => e.Nutrition).HasColumnName(FoodAnalysisResultColumnConstants.Nutrition).HasColumnType(ConfigurationConstants.JsonbColumnType);
        builder.Property(e => e.Analysis_Summary).HasColumnName(FoodAnalysisResultColumnConstants.AnalysisSummary);
        builder.OwnsOne(e => e.Serving, serving =>
        {
            serving.Property(s => s.Value).HasColumnName(FoodAnalysisResultColumnConstants.ServingValue).IsRequired();
            serving.Property(s => s.Unit).HasColumnName(FoodAnalysisResultColumnConstants.ServingUnit);
            serving.Property(s => s.Description).HasColumnName(FoodAnalysisResultColumnConstants.ServingDescription);
        });
    }
}
