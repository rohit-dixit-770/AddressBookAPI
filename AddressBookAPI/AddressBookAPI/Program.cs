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

// Register JWT Helper & Email Service
builder.Services.AddScoped<JwtTokenHelper>();
builder.Services.AddScoped<EmailService>();

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

// Session Management

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register AutoMapper

builder.Services.AddAutoMapper(typeof(AddressBookMapper));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
