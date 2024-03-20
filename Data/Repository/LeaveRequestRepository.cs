using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data.Repository
{
    public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Employee> _userManager;
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly ApplicationDbContext _db;
        public LeaveRequestRepository(ApplicationDbContext context, ILeaveAllocationRepository leaveAllocationRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, UserManager<Employee> userManager) : base(context)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _db = context;
            _leaveAllocationRepository = leaveAllocationRepository;
        }

        public async Task CancelLeaveRequest(int leaveRequestId)
        {
            var leaveRequest = await GetAsync(leaveRequestId);
            leaveRequest.Cancelled = true;
            await UpdateAsync(leaveRequest);
        }

        public async Task ChangeApprovalStatus(int leaveRequestId, bool approved)
        {
            // We need to get the leave request first
            var leaveRequest = await GetAsync(leaveRequestId);
            // Assign Approved to approve
            leaveRequest.Approved = approved;

            // If it is approved
            if(approved)
            {
                // Get the employee who's requesting the leave and what type of leave
                var allocation = await _leaveAllocationRepository.GetEmployeeAllocationLeave(leaveRequest.RequestEmployeeId, leaveRequest.LeaveTypeId);
                // Get the total number of days that is requested 
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                // Subtract the number of days the employee has after they requested the leave
                allocation.NumberOfDays -= daysRequested;

                await _leaveAllocationRepository.UpdateAsync(allocation);
            }

            await UpdateAsync(leaveRequest);
        }

        public async Task CreateLeaveRequest(LeaveRequestCreateVM leaveRequestCreateVM)
        {
            // Get the user who is logged in 
            var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);

            var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestCreateVM);
            leaveRequest.DateRequested = DateTime.Now;
            leaveRequest.RequestEmployeeId = user.Id;

            await AddAsync(leaveRequest);
        }

        public async Task<AdminLeaveRequestViewVM> GetAdminLeaveRequestList()
        {
            // Get all the leave requests
            var leaveRequests = await _db.LeaveRequests.Include(q => q.LeaveType).ToListAsync();
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequests.Count,
                ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
                PendingRequests = leaveRequests.Count(q => q.Approved == null),
                RejectedRequests = leaveRequests.Count(q => q.Approved == false),
                LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(leaveRequests),
            };

            // Get the employees who sent a leave request
            foreach (var leaveRequest in model.LeaveRequests)
            {
                leaveRequest.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveRequest.RequestEmployeeId));
            }

            return model;
        }

        public async Task<List<LeaveRequest>> GetAllAsync(string employeeId)
        {
            return await _db.LeaveRequests.Where(u=>u.RequestEmployeeId == employeeId).ToListAsync();
        }

        public async Task<LeaveRequestVM?> GetLeaveRequestAsync(int? id)
        {
            var leaveRequest = await _db.LeaveRequests.Include(u=>u.LeaveType).FirstOrDefaultAsync(u=>u.Id == id);

            if(leaveRequest == null)
            {
                return null;
            }

            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(leaveRequest?.RequestEmployeeId));
            return model;
        }

        public async Task<EmployeeLeaveRequestVM> GetMyLeaveDetails()
        {
            // Get the user who is logged in so we know who is trying to view the record
            var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);

            // Get the leave allocation so we know how many days an employee has 
            var allocation = (await _leaveAllocationRepository.GetEmployeeAllocation(user.Id)).LeaveAllocations;

            var request = _mapper.Map<List<LeaveRequestVM>>(await GetAllAsync(user.Id));

            var model = new EmployeeLeaveRequestVM(allocation, request);
            return model;
        }
    }
}