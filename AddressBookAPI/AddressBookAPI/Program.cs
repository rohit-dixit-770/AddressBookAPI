using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using AutoMapper;
using BusinessLayer.AutoMapper;
using BusinessLayer.Interface;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using BusinessLayer.Service;
using Middleware.Email;
using Middleware.JWT;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Middleware.RabbitMQ;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the database connection string

var connectionString = builder.Configuration.GetConnectionString("SqlConnection");

// Configure the DbContext to use SQL Server

builder.Services.AddDbContext<AddressBookDBContext>(options => options.UseSqlServer(connectionString));

// Register services and repositories

builder.Services.AddScoped<IAddressBookServiceBL, AddressBookServiceBL>();
builder.Services.AddScoped<IAddressBookServiceRL, AddressBookServiceRL>();

builder.Services.AddScoped<IUserBL, UserBL>();
builder.Services.AddScoped<IUserRL, UserRL>();

// Register JWT Helper, RabbitMQ & Email Service
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddTransient<RabbitMQProducer>(); 
builder.Services.AddHostedService<RabbitMQConsumer>(); 


// Add Redis configuration correctly

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (string.IsNullOrEmpty(redisConnectionString))
{
    throw new Exception("Redis connection string is missing in configuration");
}

// Add Distributed Cache with Redis

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = "AddressBookAppSession"; 
});

// ✅ Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key is missing in configuration");
}
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Session Management

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Controllers & Swagger

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Address Book API",
        Version = "v1",
        Description = "API for managing address book",
        Contact = new OpenApiContact
        {
            Name = "Rohit Dixit",
            Email = "rohitdixit570@gmail.com"
        }
    });

    // Enable JWT Authentication in Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
    // Add XML Comments for API Documentation
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


// Register AutoMapper

builder.Services.AddAutoMapper(typeof(AddressBookMapper));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();
app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
