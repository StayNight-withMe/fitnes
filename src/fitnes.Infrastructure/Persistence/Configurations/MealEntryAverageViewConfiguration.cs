using fitnes.Domain.Constants.Persistence;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fitnes.Infrastructure.Persistence.Configurations;

public static class MealEntryAverageViewConfiguration
{
    public static void ConfigureMealEntryAverageView(this EntityTypeBuilder<MealEntryAverageView> builder)
    {
        builder.HasNoKey();
        builder.ToView(ConfigurationConstants.MealEntryAveragesViewName);
        builder.Property(e => e.Id).HasColumnName(MealEntryAveragesViewColumnConstants.Id);
        builder.Property(e => e.UserId).HasColumnName(MealEntryAveragesViewColumnConstants.UserId);
        builder.Property(e => e.ConsumedAt).HasColumnName(MealEntryAveragesViewColumnConstants.ConsumedAt);
        builder.Property(e => e.Source).HasColumnName(MealEntryAveragesViewColumnConstants.Source).HasConversion<string>();
        builder.Property(e => e.DishName).HasColumnName(MealEntryAveragesViewColumnConstants.DishName);
        builder.Property(e => e.ConfidenceScore).HasColumnName(MealEntryAveragesViewColumnConstants.ConfidenceScore);
        builder.Property(e => e.AvgCalories).HasColumnName(MealEntryAveragesViewColumnConstants.AvgCalories);
        builder.Property(e => e.AvgProtein).HasColumnName(MealEntryAveragesViewColumnConstants.AvgProtein);
        builder.Property(e => e.AvgFat).HasColumnName(MealEntryAveragesViewColumnConstants.AvgFat);
        builder.Property(e => e.AvgCarbs).HasColumnName(MealEntryAveragesViewColumnConstants.AvgCarbs);
    }
}
