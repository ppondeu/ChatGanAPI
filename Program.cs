using System;
using ChatApi.Common.Filters;
using ChatApi.Data;
using ChatApi.Helpers;
using ChatApi.Interfaces;
using ChatApi.Middlewares;
using ChatApi.Repositories;
using ChatApi.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtAccessTokenMiddleware>();
builder.Services.AddScoped<JwtRefreshTokenMiddleware>();

// builder.Services.AddRouting(options =>
// {
//     options.LowercaseUrls = true;
//     options.AppendTrailingSlash = false;
// });

builder.Services.AddAuthentication().AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
        );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/users"), app =>
{
    app.UseMiddleware<JwtAccessTokenMiddleware>();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/auth/refresh-token") || context.Request.Path.StartsWithSegments("/api/auth/logout"), app =>
{
    app.UseMiddleware<JwtRefreshTokenMiddleware>();
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
