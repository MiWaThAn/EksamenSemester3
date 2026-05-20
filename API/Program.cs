using API;
using API.Workers;
using Application;
using Infrastructure.Adapters.Economic;

using Application.Interfaces.Adapters;
using Application.Interfaces.Data;
using Application;
using Application.Interfaces.Adapters;
using Application.Interfaces.Services;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Sync;
using API.Workers;
using Application.Interfaces.Services.Sync;
using Application.Services;
using Domain.Entity.Person;
using Domain.Interfaces;
using Domain.Interfaces.Item;
using Domain.Interfaces.Person;
using Domain.Services.Person;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Data;
using Infrastructure.Data.Seeding;
using Infrastructure.Service;
using Infrastructure.Service.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Adapters;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//SERVICES
//DOMAIN
builder.Services.AddTransient<ICompanyValidationService, CompanyValidationService>();
builder.Services.AddTransient<IAccountValidationService, AccountValidationService>();
builder.Services.AddTransient<IRegistrationDomainService, RegistrationDomainService>();
builder.Services.AddTransient<IAccountFactory, AccountFactory>();
builder.Services.AddTransient<ICompanyFactory, CompanyFactory>();

//INFRASTRUCTURE
var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructure(connectionstring); //FROM DEPENDENCY INJECTION IN INFRASTRUCTURE


//APPLICATION
builder.Services.AddScoped<ISyncService, SyncService>();





builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(); // Giver flottere standard-fejlformater



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


builder.Services.AddApplication();
builder.Services.AddScoped<IWebhookParser, EconomicWebhookParser>();
builder.Services.Configure<EconomicOptions>(
    builder.Configuration.GetSection(EconomicOptions.SectionName));
builder.Services.AddHttpClient<IEconomicApiClient, EconomicApiClient>();
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

//dataseeder til at smide noget data ind i vores program fra starten.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DataSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
