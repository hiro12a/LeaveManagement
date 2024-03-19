using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;

namespace LeaveManagement.Data.Repository.IRepository
{
    public interface ILeaveRequestRepository : IRepository<LeaveRequest>
    {
        Task CreateLeaveRequest(LeaveRequestCreateVM leaveRequestCreateVM);
        Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList();
        Task<EmployeeLeaveRequestVM> GetMyLeaveDetails();
    }
}