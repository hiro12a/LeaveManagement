using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    public class EditLeaveAllocationsVM : LeaveAllocationsVM
    {
        public EmployeeListVM? Employee {get;set;}
        public string? EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
    }
}