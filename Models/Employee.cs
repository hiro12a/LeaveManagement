using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagement.Models
{
    public class Employee : IdentityUser
    {
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DOB { get; set; } 
        public DateTime JoinDate { get; set; }
    }
}