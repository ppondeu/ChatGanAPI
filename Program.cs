using ChatApi.Common.Filters;
using ChatApi.Data;
using ChatApi.Helpers;
using ChatApi.Hubs;
using ChatApi.Interfaces;
using ChatApi.Middlewares;
using ChatApi.Repositories;
using ChatApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Scalar.AspNetCore;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSingleton<IRedisService, RedisService>();

// Add services to the container.
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>(); // singleton
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IFileService, FileService>(); // add transient
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IJwtService, JwtService>(); // singleton
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtAccessTokenMiddleware>(); // add transient
builder.Services.AddScoped<JwtRefreshTokenMiddleware>(); //add transient

// builder.Services.AddRouting(options =>
// {
//     options.LowercaseUrls = true;
//     options.AppendTrailingSlash = false;
// });

builder.Services.AddAuthentication().AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173", "http://172.19.160.1:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddSignalR();

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

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/users") || context.Request.Path.StartsWithSegments("/api/chats"), app =>
{
    app.UseMiddleware<JwtAccessTokenMiddleware>();
});

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/auth/refresh-token") || context.Request.Path.StartsWithSegments("/api/auth/logout"), app =>
{
    app.UseMiddleware<JwtRefreshTokenMiddleware>();
});


app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Attachments"
});

app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
