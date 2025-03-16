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

// Register AutoMapper

builder.Services.AddAutoMapper(typeof(AddressBookMapper));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
