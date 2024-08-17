using LeaveManagement.Data;
using LeaveManagement.Data.Repository;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.SecretManager.V1;
using Google.Apis.Auth.OAuth2;


var builder = WebApplication.CreateBuilder(args);

// Configure the Secret Manager client
var client = SecretManagerServiceClient.Create();
string projectId = "ksortreeservice-414322"; // Replace with your actual project ID
string googleSecretId = "GOOGLESECRETSCREDENTIAL"; // Your Google Cloud secret ID
string dbSecretId = "LeaveManagerDB"; // Your database connection string secret ID

try
{
    // Access the Google credentials secret
    var googleSecretVersionName = new SecretVersionName(projectId, googleSecretId, "latest");
    var googleSecretVersion = client.AccessSecretVersion(googleSecretVersionName);
    var googleSecretPayload = googleSecretVersion.Payload.Data.ToStringUtf8();
    
    var googleCredential = GoogleCredential.FromJson(googleSecretPayload);

    // Access the database connection string secret
    var dbSecretVersionName = new SecretVersionName(projectId, dbSecretId, "latest");
    var dbSecretVersion = client.AccessSecretVersion(dbSecretVersionName);
    var dbSecretPayload = dbSecretVersion.Payload.Data.ToStringUtf8();

    // Register the DbContext with the retrieved connection string
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(dbSecretPayload));

    // Use GoogleCredential if needed
    // For example, configure Google Cloud services that require authentication
    // You can pass googleCredential to other services or clients as needed

}
catch (Exception ex)
{
    // Handle exceptions when retrieving secrets
    Console.WriteLine($"Error retrieving secret: {ex.Message}");
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add other services and configurations as needed
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
