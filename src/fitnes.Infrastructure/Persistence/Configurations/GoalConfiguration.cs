using fitnes.Domain.Constants.Persistence;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fitnes.Infrastructure.Persistence.Configurations;

public static class GoalConfiguration
{
    public static void ConfigureGoal(this EntityTypeBuilder<UserGoal> builder)
    {
        builder.ToTable(ConfigurationConstants.UserGoalTableName);
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName(GoalColumnConstants.Id).HasDefaultValueSql(ConfigurationConstants.GenRandomUuidDefaultSql);
        builder.Property(e => e.UserId).HasColumnName(GoalColumnConstants.UserId).IsRequired();
        builder.Property(e => e.Type).HasColumnName(GoalColumnConstants.Type).IsRequired();
        builder.Property(e => e.TargetWeight).HasColumnName(GoalColumnConstants.TargetWeight).HasColumnType("double precision").IsRequired();
        builder.Property(e => e.Activity).HasColumnName(GoalColumnConstants.Activity).IsRequired();
        builder.Property(e => e.StartWeight).HasColumnName(GoalColumnConstants.StartWeight).HasColumnType("double precision").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName(GoalColumnConstants.CreatedAt).HasColumnType(ConfigurationConstants.TimestamptzColumnType).IsRequired().HasDefaultValueSql(ConfigurationConstants.NowDefaultSql);
        builder.Property(e => e.IsActive).HasColumnName(GoalColumnConstants.IsActive).IsRequired();
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(e => new { e.UserId, e.IsActive });
    }
}
