using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

using API;
using Domain.Entity.Person;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Application.Workers;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Giver flottere standard-fejlformater
builder.Services.AddScoped<IPasswordHasher<Account>,PasswordHasher<Account>>();
builder.Services.AddTransient<IAccountFactory, AccountFactory>();
builder.Services.AddTransient<ICompanyFactory, CompanyFactory>();
builder.Services.AddTransient<ICompanyValidationService, CompanyValidationService>();
builder.Services.AddTransient<IAccountValidationService, AccountValidationService>();
builder.Services.AddHostedService<SyncWithExternalWorker>();
builder.Services.AddHttpClient();
//Adds Infrastructure repos and so on.
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionstring);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
