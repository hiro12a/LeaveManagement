using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    public class EmployeeLeaveRequestVM
    {
        public EmployeeLeaveRequestVM(List<EmployeeLeaveRequestVM> leaveAllocations, List<LeaveRequestVM> leaveRequests)
        {
            LeaveAllocations = leaveAllocations;
            LeaveRequests = leaveRequests;
        }    
        public List<EmployeeLeaveRequestVM> LeaveAllocations { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
}