using System;
using System.Collections.Generic;
using System.Text;
using fitnes.Domain.Options;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fitnes.bot.Extension.DI;

public static class OptionExtensions
{
    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeyOptions>(configuration.GetSection(ApiKeyOptions.SectionName));
        services.Configure<BotOptions>(configuration.GetSection(BotOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));
        services.Configure<SessionOptions>(configuration.GetSection(SessionOptions.SectionName));

        return services;
    }
}
