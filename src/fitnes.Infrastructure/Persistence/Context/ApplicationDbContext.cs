using fitnes.Domain.Entities;
using fitnes.Domain.Models.Fitnes;
using fitnes.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace fitnes.Infrastructure.Persistence.Context;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<MealEntry> MealEntries { get; set; } = default!;
    public DbSet<FoodAnalysisResult> FoodAnalysisResults { get; set; } = default!;
    public DbSet<UserGoal> UserGoals { get; set; } = default!;
    public DbSet<MealEntryAverageView> MealEntryAverages { get; set; } = default!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().ConfigureUser();
        modelBuilder.Entity<MealEntry>().ConfigureMealEntry();
        modelBuilder.Entity<FoodAnalysisResult>().ConfigureFoodAnalysisResult();
        modelBuilder.Entity<UserGoal>().ConfigureGoal();
        modelBuilder.Entity<MealEntryAverageView>().ConfigureMealEntryAverageView();
    }
}
