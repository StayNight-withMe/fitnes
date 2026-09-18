using fitnes.Domain.Abstraction.Common;
using fitnes.Infrastructure.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace fitnes.bot.Extension.DI;

public static class LocalizationExtensions
{
    public static IServiceCollection AddLocalization(this IServiceCollection services)
    {
        services.AddScoped<ILocalizer, Localizer>();
        services.AddSingleton<LocalizationProvider>();
        return services;
    }
}
