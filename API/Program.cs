using API;
using API.Workers;
using Application;
using Application.Adapters;
using Application.Adapters.Economic;
using Application.Interfaces.Adapters;
using Application.Interfaces.Data;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Sync;
using Application.Services;
using Domain.Entity.Person;
using Domain.Interfaces;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces.Services;
using Infrastructure.Service.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Giver flottere standard-fejlformater
builder.Services.AddScoped<IPasswordHasher<Account>,PasswordHasher<Account>>();
builder.Services.AddTransient<IAccountFactory, AccountFactory>();
builder.Services.AddTransient<ICompanyFactory, CompanyFactory>();
builder.Services.AddTransient<ICompanyValidationService, CompanyValidationService>();
builder.Services.AddTransient<IAccountValidationService, AccountValidationService>();
builder.Services.AddTransient<IRegistrationDomainService, RegistrationDomainService>();
builder.Services.AddHostedService<SyncWithExternalWorker>();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IExternalAPIService, ExternalAPIService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IProviderAdapter, EconomicAdapter>();
builder.Services.AddScoped<AdapterRegistry>();

//Adds Infrastructure repos and so on.
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionstring);
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
