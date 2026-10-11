using Uptime.Web.Data.Migrations;
using Uptime.Web.Extensions;
using Uptime.Web.Routes;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder
    .Services.AddRespositories()
    .AddServices(builder.Configuration)
    .AddBackgroundServices()
    .AddMigrations()
    .ConfigureOptions(builder.Configuration);

var app = builder.Build();

await app.MigrateDatabase();

app.MapGet(HomeRoute.Route, HomeRoute.HandleAsync);

app.Run();
