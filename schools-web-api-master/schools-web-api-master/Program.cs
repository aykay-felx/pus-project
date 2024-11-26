using System;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using schools_web_api.TokenManager;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using schools_web_api.TokenManager.Services.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using schools_web_api.TokenManager.Services.Implementation;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
     {
         options.AddPolicy("AllowAll",
             builder =>
             {
                 builder
                 .AllowAnyOrigin() 
                 .AllowAnyMethod()
                 .AllowAnyHeader();
             });
     });
    
builder.Services.AddSingleton<ISchoolService, SchoolService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<ITokenManager, TokenManager>();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Schools api", Version = "v1" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new 
        List<string> { "index.html" }
    }
);

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();