using LeaveManagement.Data;
using LeaveManagement.Data.Repository;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Google.Cloud.SecretManager.V1;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Configure Google Cloud Secret Manager Client
string credentialPath = "Data/ksortreeservice-414322-4a6b6e064aa0.json"; // Update to your actual path
var credential = GoogleCredential.FromFile(credentialPath);

// Create the Secret Manager client with the credentials
var secretManagerClient = new SecretManagerServiceClientBuilder
{
    // Use the credential directly with SecretManagerServiceClientBuilder
    CredentialsPath = credentialPath
}.Build();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure and register the SecretManagerService
builder.Services.AddSingleton<SecretManagerService>();

// Configure the DbContext by fetching the secret asynchronously
var secretService = builder.Services.BuildServiceProvider().GetRequiredService<SecretManagerService>();
string projectId = "ksortreeservice-414322"; // Replace with your actual project ID
string secretId = "LeaveManagerDB"; // The name of your secret

try
{
    string connectionString = await secretService.GetSecretAsync(projectId, secretId);

    // Register the DbContext with the retrieved connection string
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    // Handle exceptions when retrieving secrets
    Console.WriteLine($"Error retrieving secret: {ex.Message}");
}

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
