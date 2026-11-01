using Uptime.Web.Services;

namespace Uptime.Web.Extensions;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<Migrator>();
        await migrator.MigrateAsync();
        return app;
    }
}
