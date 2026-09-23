
using fitnes.Application.Services;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Services;
using fitnes.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace fitnes.bot.Extension.DI;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpClient<ICalorieService, GeminiCalorieUtils>()
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                ConnectTimeout = TimeSpan.FromSeconds(30),
            });
        services.AddScoped<ISessionService, SessionService>();

        return services;
    }
}
