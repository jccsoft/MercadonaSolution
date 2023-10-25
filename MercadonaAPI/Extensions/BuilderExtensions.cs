using MercadonaAPI.Clients;
using MercadonaAPI.Helpers;
using MercadonaAPI.Options;
using MercadonaAPI.Workers;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Compact;

namespace MercadonaAPI.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddMercadonaSetup(this WebApplicationBuilder builder)
    {
        MercadonaOptions options = new();
        var section = builder.Configuration.GetSection(MercadonaOptions.SettingsName);
        builder.Services.Configure<MercadonaOptions>(section);

        section.Bind(options);
        builder.Services.AddHttpClient(MercadonaOptions.HttpClientName, (serviceProvider, client) =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<MercadonaOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        builder.Services.AddSingleton<MercadonaClient>();

        if (options.ActiveWorkerService && options.BaseUrl.ToLower().Contains("localhost") == false)
        {
            builder.Services.AddHostedService<MercadonaWorker>();
        }

        return builder;
    }

    public static WebApplicationBuilder AddSerilogSetup(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.With(new DateTimeEnricher())
                    .WriteTo.Console());

        return builder;
    }
}
