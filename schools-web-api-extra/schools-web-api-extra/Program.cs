using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Включение Swagger только в среде разработки
if (app.Environment.IsProduction())
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
