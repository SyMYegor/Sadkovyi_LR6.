using AgroMaintenance.Api.Configuration;
using AgroMaintenance.Api.Data;
using AgroMaintenance.Api.Filters;
using AgroMaintenance.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

// Завдання 2,4,5. Реєстрація API, конфігурації, Swagger та БД.
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddOptions<ApiSettings>()
    .Bind(builder.Configuration.GetSection("ApiSettings"))
    .ValidateDataAnnotations()
    .Validate(
        settings =>
            !string.IsNullOrWhiteSpace(settings.DatabaseName)
            && settings.DatabaseName.Trim().Length >= 4,
        "Некоректна назва бази даних")
    .ValidateOnStart();

builder.Services.AddDbContext<AgroDbContext>((services, options) =>
{
    var settings = services
        .GetRequiredService<IOptions<ApiSettings>>()
        .Value;

    options.UseInMemoryDatabase(settings.DatabaseName);
});

builder.Services.AddScoped<RequestRules>();
builder.Services.AddScoped<ApiExceptionFilter>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Завдання 7. Автоматична відповідь 400 та журнал валідації.
        options.InvalidModelStateResponseFactory = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("Validation");

            var messages = context.ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error =>
                    string.IsNullOrEmpty(error.ErrorMessage)
                        ? "Некоректний JSON."
                        : error.ErrorMessage);

            logger.LogWarning(
                "Помилка валідації: {Messages}",
                string.Join("; ", messages));

            return new BadRequestObjectResult(
                new ValidationProblemDetails(context.ModelState));
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    // Завдання 4. Перевірка параметрів до запуску сервера.
    var validatedSettings = app.Services
        .GetRequiredService<IOptions<ApiSettings>>()
        .Value;

    // Завдання 5. Початкове заповнення БД.
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider
            .GetRequiredService<AgroDbContext>();

        SeedData.Initialize(db, app.Environment.ContentRootPath);
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();
    app.Run();
}
catch (OptionsValidationException exception)
{
    Console.WriteLine(
        "Помилка: Некоректна конфігурація ApiSettings. " +
        "Перевірте назву бази даних і MaxBulkSize.");

    foreach (var failure in exception.Failures)
    {
        Console.WriteLine(failure);
    }

    Environment.ExitCode = 1;
}