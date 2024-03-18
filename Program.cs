using APIx.Config;
using APIx.Data;
using APIx.Helpers;
using APIx.Middlewares;
using APIx.OCMinimal;
using APIx.Repositories;
using APIx.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Prometheus;

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

// Services
builder.Services.AddScoped<KeysService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PaymentsService>();
builder.Services.AddScoped<MessageService>();

// Repositories
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<KeysRepository>();
builder.Services.AddScoped<AccountsRepository>();
builder.Services.AddScoped<PaymentsRepository>();

// Rabbimq Queue
IConfigurationSection queueConfig = builder.Configuration.GetSection("QueueSettings");
builder.Services.Configure<QueueConfig>(queueConfig);

// Cache
builder.Services.AddStackExchangeRedisCache(options =>
    {
        string host = builder.Configuration["Cache:Host"] ?? string.Empty;
        string port = builder.Configuration["Cache:Port"] ?? string.Empty;
        string instance = builder.Configuration["Cache:Instance"] ?? string.Empty;
        options.Configuration = $"{host}:{port}";
        options.InstanceName = instance;
    });
    
builder.Services.AddOutputCache(opt => 
{
    opt.AddPolicy("CacheAuthenticated", MyCustomPolicy.Instance);
});
builder.Services.AddStackExchangeRedisOutputCache(opt =>
    {
        string host = builder.Configuration["Cache:Host"] ?? string.Empty;
        string port = builder.Configuration["Cache:Port"] ?? string.Empty;
        string instance = builder.Configuration["Cache:Instance"] ?? string.Empty;
        opt.Configuration = $"{host}:{port}";
        opt.InstanceName = instance;
    });


// Authentication
builder.Services.AddAuthentication("BearerAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("BearerAuthentication", null);

var app = builder.Build();

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

// Cache
app.UseOutputCache();

app.Run();
