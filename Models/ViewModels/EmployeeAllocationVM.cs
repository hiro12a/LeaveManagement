using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    // We want to inherit from EmployeeListVM
    public class EmployeeAllocationVM : EmployeeListVM
    {
        public List<LeaveAllocationsVM>? LeaveAllocations{get;set;}
    }
}