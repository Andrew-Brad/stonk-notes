using System.Reflection;
using MediatR.Pipeline;
using StonkNotes.Application.Common.Behaviours;
using StonkNotes.Common;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var appLayerAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .Single(a => a.GetCustomAttribute<ApplicationLayerAssemblyAttribute>() != null);

        services.AddAutoMapper(appLayerAssembly);

        services.AddValidatorsFromAssembly(appLayerAssembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(appLayerAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            //cfg.AutoRegisterRequestProcessors = true;
            cfg.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionLoggingBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        return services;
    }
}
