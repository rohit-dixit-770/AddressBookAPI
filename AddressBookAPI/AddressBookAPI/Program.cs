using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using AutoMapper;
using BusinessLayer.AutoMapper;
using BusinessLayer.Interface;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using BusinessLayer.Service;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the database connection string

var connectionString = builder.Configuration.GetConnectionString("SqlConnection");

// Configure the DbContext to use SQL Server

builder.Services.AddDbContext<AddressBookDBContext>(options => options.UseSqlServer(connectionString));

// Register services and repositories

builder.Services.AddScoped<IAddressBookServiceBL, AddressBookServiceBL>();
builder.Services.AddScoped<IAddressBookServiceRL, AddressBookServiceRL>();

// Register AutoMapper

builder.Services.AddAutoMapper(typeof(AddressBookMapper));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
