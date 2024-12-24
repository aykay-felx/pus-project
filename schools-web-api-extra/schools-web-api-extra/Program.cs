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

app.MapControllers();

app.Run();
