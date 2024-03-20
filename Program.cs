using APIx.Config;
using APIx.Data;
using APIx.Helpers;
using APIx.Helpers.RabbitMQ;
using APIx.Middlewares;
using APIx.Repositories;
using APIx.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using Microsoft.OpenApi.Models;
using Prometheus;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
builder.Services.AddDbContext<AppDBContext>(opts =>
{
    string host = builder.Configuration["Database:Host"] ?? string.Empty;
    string port = builder.Configuration["Database:Port"] ?? string.Empty;
    string username = builder.Configuration["Database:Username"] ?? string.Empty;
    string database = builder.Configuration["Database:Name"] ?? string.Empty;
    string password = builder.Configuration["Database:Password"] ?? string.Empty;

    string connectionString = $"Host={host};Port={port};Username={username};Password={password};Database={database}";
    opts.UseNpgsql(connectionString);
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new() { Title = "APIx", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Rabbimq Queue
IConfigurationSection RabbitOptions = builder.Configuration.GetSection("QueueSettings");
builder.Services.Configure<RabbitOptions>(RabbitOptions);
builder.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
builder.Services.AddSingleton<IPooledObjectPolicy<IModel>, ChannelPooledObjectPolicy>();
builder.Services.AddSingleton<IRabbitManager, RabbitManager>();

// Cache
builder.Services.AddStackExchangeRedisCache(options =>
    {
        string host = builder.Configuration["Cache:Host"] ?? string.Empty;
        string port = builder.Configuration["Cache:Port"] ?? string.Empty;
        options.Configuration = $"{host}:{port}";
    });

// Authentication
builder.Services.AddAuthentication("BearerAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("BearerAuthentication", null);

// Services
builder.Services.AddScoped<KeysService>();
builder.Services.AddScoped<PaymentsService>();

// Repositories
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<KeysRepository>();
builder.Services.AddScoped<AccountsRepository>();
builder.Services.AddScoped<PaymentsRepository>();
builder.Services.AddScoped<CacheRepository>();

// Helpers

var app = builder.Build();

// // Queue
// var rabbitManager = app.Services.GetRequiredService<IRabbitManager>();
// rabbitManager.QueueDeclare("payments", true, false, false, null);

// Monitoring and Metrics
app.UseMetricServer();
app.UseHttpMetrics(options => options.AddCustomLabel("host", context => context.Request.Host.Host));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapMetrics();

// Middlewares
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
