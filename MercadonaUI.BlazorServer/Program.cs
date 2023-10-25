using MercadonaUI.BlazorServer.Extensions;
using MercadonaUI.BlazorServer.Helpers;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);

    builder.AddSerilogSetup();


    // Add services to the container.
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();

    builder.Services.AddMudServices();


    builder.AddMercadonaSetup();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();

    }

    app.UseHttpsRedirection();


    app.UseStaticFiles();

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}