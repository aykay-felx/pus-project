using Microsoft.OpenApi.Models;
using schools_web_api_extra.Interface;
using schools_web_api_extra.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Schools API Extra",
        Version = "v1",
        Description = "API for updating data from RSPO database"
    });
});

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddSingleton<ISchoolService, SchoolRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(origin => 
            new Uri(origin).Host == "localhost") // Allow any localhost regardless of port
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Swagger is being set up");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schools API Extra v1");
    });
}

app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();
