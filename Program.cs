using Google.Cloud.SecretManager.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Data.Repository;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using LeaveManagement.Models;
using LeaveManagement.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Google Cloud Secret Manager Client
var googleCredentialsFilePath = builder.Configuration.GetValue<string>("GoogleCloud:CredentialsFilePath");

// Parse the JSON string to create the GoogleCredential object
if (!File.Exists(googleCredentialsFilePath))
{
    throw new Exception($"Google credentials file not found at path: {googleCredentialsFilePath}");
}

// Parse the JSON file to create the GoogleCredential object
var credential = GoogleCredential.FromFile(googleCredentialsFilePath);

// Create the Secret Manager client using the credentials
var secretManagerClient = new SecretManagerServiceClientBuilder
{
    Credential = credential
}.Build();

// Register the Secret Manager client as a singleton
builder.Services.AddSingleton(secretManagerClient);

// Register the DbContext with a factory method to handle async operations
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(GetConnectionString().Result);
});

async Task<string> GetConnectionString()
{
    string projectId = "ksortreeservice-414322"; // Replace with your actual project ID
    string secretId = "LeaveManagerDB"; // The name of your secret

    try
    {
        var secretVersionName = new SecretVersionName(projectId, secretId, "latest");
        var secret = await secretManagerClient.AccessSecretVersionAsync(secretVersionName);
        return secret.Payload.Data.ToStringUtf8();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving secret: {ex.Message}");
        throw;
    }
}

// Add services to the container
builder.Services.AddControllersWithViews();

// For Identity. Allows us to add extra field to IdentityUser
builder.Services.AddIdentity<Employee, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Automapper
builder.Services.AddAutoMapper(typeof(Mapper));

// Allows us to use IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
builder.Services.AddScoped<ILeaveAllocationRepository, LeaveAllocationRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

// Configure application cookie
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = $"/Employee/AccessDenied";
    option.LoginPath = $"/Employee/Index";
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ensure this is called before Authorization
app.UseAuthorization();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

app.Run();
