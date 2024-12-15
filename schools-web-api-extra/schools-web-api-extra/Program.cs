using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавление конфигурации из appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Добавление Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Schools API Extra",
        Version = "v1",
        Description = "API для интеграции с RSPO"
    });
});

// Добавление HttpClientFactory для использования в контроллерах
builder.Services.AddHttpClient();

// Добавление контроллеров
builder.Services.AddControllers();

// Регистрация DatabaseHelper с DI
builder.Services.AddSingleton<DatabaseHelper>();

var app = builder.Build();

// Включение Swagger только в среде разработки
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Swagger is being set up");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schools API Extra v1");
    });
}

// Настройка маршрутов
app.MapControllers();

app.Run();
