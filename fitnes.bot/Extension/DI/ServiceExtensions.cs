
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
        services.AddScoped<ICalorieService, GeminiCalorieUtils>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<HttpClient>();

        return services;
    }
}
