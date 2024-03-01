using LeaveManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<Employee>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}

        public DbSet<Employee> employees { get; set; }
        public DbSet<LeaveAllocations> leaveAllocations {get;set;}
        public DbSet<LeaveType> leaveTypes {get;set;}
    }
}