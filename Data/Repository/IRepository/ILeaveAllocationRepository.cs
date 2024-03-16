using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;

namespace LeaveManagement.Data.Repository.IRepository
{
    public interface ILeaveAllocationRepository : IRepository<LeaveAllocations>
    {
        Task LeaveAllocation(int leaveTypeId);
        Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period);
        Task<EmployeeAllocationVM> EmployeeAllocationVM(string employeeId);
    }
}