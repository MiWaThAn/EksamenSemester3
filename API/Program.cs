using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;

using API;
using Domain.Entity.Person;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Application.Workers;
using Application.Interfaces.Services;
using Infrastructure.Service.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
//Adds Infrastructure repos and so on.
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionstring);

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
