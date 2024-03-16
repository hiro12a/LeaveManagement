using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    public class LeaveAllocations : BaseEntity
    {
        // Number of days the employee wants to leave 
        public int NumberOfDays {get;set;}

        // Connect LeaveType to leave allocation so employee can select what leave type they want
        public int LeaveTypeId {get;set;}
        [ForeignKey(nameof(LeaveTypeId))]
        public LeaveType? LeaveType {get;set;}

        public string? EmployeeId {get;set;}

        public int Period {get;set;}
    }
}