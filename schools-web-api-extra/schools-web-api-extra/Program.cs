using Microsoft.OpenApi.Models;
using schools_web_api;
using schools_web_api.TokenManager.Services.Implementation;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager;
using schools_web_api_extra.Interface;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddControllers();
builder.Services.AddSingleton<ISchoolService, SchoolService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITokenManager, TokenManager>();
builder.Services.AddScoped<INewService, NewSchoolService>();

// ���������� Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Schools API Extra",
        Version = "v1",
        Description = "API ��� ���������� � RSPO"
    });
});

// ���������� HttpClientFactory ��� ������������� � ������������
builder.Services.AddHttpClient();

// ���������� ������������
builder.Services.AddControllers();

var app = builder.Build();

// ��������� Swagger ������ � ����� ����������
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Swagger is being set up");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schools API Extra v1");
    });
}

// ��������� ���������
app.MapControllers();

app.Run();
