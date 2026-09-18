using fitnes.Application;
using fitnes.Domain.Abstraction.Common;
using fitnes.Infrastructure.Behaviors;
using fitnes.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace fitnes.bot.Extension.DI;

public static class PipelineExtensions
{
    public static IServiceCollection AddPipeline(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(SessionBehavior<,>));
        });

        services.AddScoped<RequestContext>();
        services.AddScoped<IRequestContext>(sp => sp.GetRequiredService<RequestContext>());

        return services;
    }
}
