using fitnes.bot.Abstractions;
using fitnes.bot.Decorators;
using fitnes.bot.Handlers.Calories;
using fitnes.bot.Handlers.Goals;
using fitnes.bot.Handlers.Languages;
using fitnes.bot.Handlers.Meals;
using fitnes.bot.Handlers.MoveBack;
using fitnes.bot.Handlers.Profile;
using fitnes.bot.Handlers.Start;
using fitnes.bot.Handlers.StartWork;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace fitnes.bot.Extension.DI;

public static class HandlerExtensions
{
    public static IServiceCollection AddBotHandlers(this IServiceCollection services)
    {
        services.AddScoped<BotUpdateHandler>();
        services.AddScoped<IBotUpdateHandler>(sp => new BotUpdateHandlerDecorator(
            sp.GetRequiredService<BotUpdateHandler>(),
            sp.GetRequiredService<ISessionRepository>(),
            sp.GetRequiredService<ISessionService>(),
            sp.GetRequiredService<IBaseRepository<User, long>>(),
            sp.GetRequiredService<RequestContext>(),
            sp.GetRequiredService<ITelegramBotClient>(),
            sp.GetRequiredService<ILogger<BotUpdateHandlerDecorator>>()
        ));

        services.AddScoped<IBotHandler, StartCommandHandler>();
        services.AddScoped<IBotHandler, LanguageSetHandler>();
        services.AddScoped<IBotHandler, CalorieCalculationHandler>();
        services.AddScoped<IBotHandler, MealSaveHandler>();
        services.AddScoped<IBotHandler, MealStatsHandler>();
        services.AddScoped<IBotHandler, GoalsMenuHandler>();
        services.AddScoped<IBotHandler, GoalActionHandler>();
        services.AddScoped<IBotHandler, GoalsInputHandler>();
        services.AddScoped<IBotHandler, CalorieCalculationInfoHadnler>();
        services.AddScoped<IBotHandler, LanguageSelectHandler>();
        services.AddScoped<IBotHandler, StartWorkCallBackHandler>();
        services.AddScoped<IBotHandler, MoveBackHandler>();
        services.AddScoped<IBotHandler, CancelHandler>();
        services.AddScoped<IBotHandler, ProfileViewHandler>();
        services.AddScoped<IBotHandler, ProfileEditHandler>();
        services.AddScoped<IBotHandler, ProfileWeightHandler>();
        services.AddScoped<IBotHandler, WeightInputHandler>();
        services.AddScoped<IBotHandler, ProfileHandler>();

        return services;
    }
 }
