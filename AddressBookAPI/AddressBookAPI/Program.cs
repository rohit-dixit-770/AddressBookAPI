using AddressBookAPI.AutoMapper;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the database connection string
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");

// Configure the DbContext to use SQL Server
builder.Services.AddDbContext<AddressBookDBContext>(options => options.UseSqlServer(connectionString));

// Register AutoMapper

builder.Services.AddAutoMapper(typeof(AddressBookMapper));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
