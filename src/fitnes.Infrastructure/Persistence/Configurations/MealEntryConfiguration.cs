using fitnes.Domain.Constants.Persistence;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fitnes.Infrastructure.Persistence.Configurations;

public static class MealEntryConfiguration
{
    public static void ConfigureMealEntry(this EntityTypeBuilder<MealEntry> builder)
    {
        builder.ToTable(ConfigurationConstants.MealLogTableName);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName(MealEntryColumnConstants.Id).HasDefaultValueSql(ConfigurationConstants.GenRandomUuidDefaultSql);
        builder.Property(e => e.UserId).HasColumnName(MealEntryColumnConstants.UserId).IsRequired();
        builder.Property(e => e.ConsumedAt).HasColumnName(MealEntryColumnConstants.ConsumedAt).HasColumnType(ConfigurationConstants.TimestamptzColumnType).IsRequired().HasDefaultValueSql(ConfigurationConstants.NowDefaultSql);
        builder.Property(e => e.Source).HasColumnName(MealEntryColumnConstants.Source).IsRequired().HasConversion<string>();
        builder.Property(e => e.AnalysisResultId).HasColumnName(MealEntryColumnConstants.AnalysisResultId);
        builder.HasOne(e => e.User).WithMany(u => u.Meals).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.AnalysisResult).WithOne().HasForeignKey<MealEntry>(e => e.AnalysisResultId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(e => new { e.UserId, e.ConsumedAt }).IsDescending(false, true);
    }
}
