using Uptime.Web.BackgroundServices;
using Uptime.Web.Data.Repositories;
using Uptime.Web.Services;

namespace Uptime.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<UrlResolver>();
        services.AddSingleton<Migrator>();
        services.AddSingleton<VersionProvider>();
        services.AddSingleton(sp =>
            PingServiceFactory.Create(sp.GetRequiredService<UrlResolver>())
        );
        return services;
    }

    public static IServiceCollection AddRespositories(this IServiceCollection services)
    {
        services.AddSingleton<PingResultsRepository>();
        services.AddSingleton<OutagesRepository>();
        return services;
    }

    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<PingHostedService>();
        return services;
    }

    public static IServiceCollection ConfigureOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<HostOptions>(o =>
            o.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore
        );
        return services;
    }
}
