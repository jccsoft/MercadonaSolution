using MercadonaAPI.Extensions;
using MercadonaAPI.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogSetup();


// Add services to the container.
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.AddMercadonaSetup();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowOrigin");

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
