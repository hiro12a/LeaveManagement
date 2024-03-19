using LeaveManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveAllocations> LeaveAllocations {get;set;}
        public DbSet<LeaveType> LeaveTypes {get;set;}
        public DbSet<LeaveRequest> LeaveRequests {get;set;}
    }
}