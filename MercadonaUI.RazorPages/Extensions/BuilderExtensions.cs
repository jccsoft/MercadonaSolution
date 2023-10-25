using MercadonaUI.RazorPages.Helpers;
using MercadonaUI.RazorPages.Options;
using Microsoft.Extensions.Options;
using Serilog;

namespace MercadonaUI.RazorPages.Extensions;

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
            client.BaseAddress = new Uri(options.AzureBaseUriString);
        });

        builder.Services.AddScoped<ImagesHelper>();

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
