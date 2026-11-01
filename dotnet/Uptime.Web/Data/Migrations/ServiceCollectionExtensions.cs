namespace Uptime.Web.Data.Migrations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services.AddSingleton<IMigration, Migration_0001_AddPingResult>();
        services.AddSingleton<IMigration, Migration_0002_AddOutage>();
        return services;
    }
}
