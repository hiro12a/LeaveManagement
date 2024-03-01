using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Models
{
    public class LeaveType : BaseEntity
    {
        public string? Name { get; set; }

        // Default number of days an employee get 
        public int DefaultDays { get; set; }
    }
}