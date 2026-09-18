using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Domain.Options;
using fitnes.Infrastructure.Persistence.Context;
using fitnes.Infrastructure.Persistence.Repositories.Base;
using fitnes.Infrastructure.Persistence.Repositories.Target;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using StackExchange.Redis;

namespace fitnes.bot.Extension.DI;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseOptions = configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>();

        if (databaseOptions is null)
        {
            throw new InvalidOperationException(nameof(DatabaseOptions));
        }

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(databaseOptions.PostgresConnection);

        dataSourceBuilder.EnableDynamicJson();

        services.AddSingleton(dataSourceBuilder.Build());

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseNpgsql(serviceProvider.GetRequiredService<NpgsqlDataSource>());
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(databaseOptions.RedisConnection);
        });


        services.AddScoped<ISessionRepository, RedisSessionRepository>();

        services.AddScoped(typeof(IBaseRepository<,>), typeof(PgBaseRepository<,>));
        services.AddScoped<IBaseRepository<User, long>, UserPgRepository>();
        services.AddScoped<IMealEntryRepository, MealEntryPgRepository>();
        services.AddScoped<IGoalRepository, GoalPgRepository>();

        return services;
    }
}

