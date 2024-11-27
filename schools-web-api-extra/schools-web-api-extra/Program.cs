using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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
if (app.Environment.IsProduction())
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
