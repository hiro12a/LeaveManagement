using LeaveManagement.Data;
using LeaveManagement.Data.Repository;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


// Connect to database
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnections")));

// For Identity. Allows us to add extra field to identityUser
builder.Services.AddIdentity<Employee, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// Automapper
builder.Services.AddAutoMapper(typeof(Mapper));

// Allows us to use IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Reference Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
builder.Services.AddScoped<ILeaveAllocationRepository, LeaveAllocationRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();

// Redirect
builder.Services.ConfigureApplicationCookie(option =>
{
    option.AccessDeniedPath = $"/Employee/AccessDenied";
    option.LoginPath = $"/Employee/Index";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
