using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ���������� ������������ �� appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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

// ����������� DatabaseHelper � DI
builder.Services.AddSingleton<DatabaseHelper>();

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
