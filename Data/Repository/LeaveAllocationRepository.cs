using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data.Repository
{
    public class LeaveAllocationRepository : Repository<LeaveAllocations>, ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<Employee> _userManager;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;

        public LeaveAllocationRepository(ApplicationDbContext context, 
            UserManager<Employee> userManager, 
            ILeaveTypeRepository leaveTypeRepository,
            IMapper mapper) : base(context)
        {
            _db = context;
            _userManager = userManager;
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
        }


        public async Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period)
        {
            return await _db.LeaveAllocations.AnyAsync(u => u.EmployeeId == employeeId && u.LeaveTypeId == leaveTypeId && u.Period == period);
        }

        public async Task<EmployeeAllocationVM> EmployeeAllocationVM(string employeeId)
        {
            // Lambda expression to get the LeaveType and leave allocation
            // Include makes sure we get data from leaveType
            var allocations = await _db.LeaveAllocations.Include(q => q.LeaveType).Where(q => q.EmployeeId == employeeId).ToListAsync();

            // get employee details
            var employeeAllocation = await _userManager.FindByIdAsync(employeeId);

            // Map employee and allocations
            var employeeAllocationModel = _mapper.Map<EmployeeAllocationVM>(employeeAllocation);
            employeeAllocationModel.LeaveAllocations = _mapper.Map<List<LeaveAllocationsVM>>(allocations);

            return employeeAllocationModel;
        }

        public async Task<EditLeaveAllocationsVM> GetEmployeeAllocation(int id)
        {
            var allocation = await _db.LeaveAllocations.Include(q => q.LeaveType).FirstOrDefaultAsync(q => q.Id == id);

            // get employee details
            if(allocation == null){
                return null;
            }
            
            // Map employee and allocations
            var employee = await _userManager.FindByIdAsync(allocation.EmployeeId);
            var model = _mapper.Map<EditLeaveAllocationsVM>(allocation);
            model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(allocation.EmployeeId));

            return model;
        }

        public async Task LeaveAllocation(int leaveTypeId)
        {
            var employees = await _userManager.GetUsersInRoleAsync(SD.Role_Employee);
            var period = DateTime.Now.Year;
            var leaveType = await _leaveTypeRepository.GetAsync(leaveTypeId);
            var allocations = new List<LeaveAllocations>();

            foreach (var employee in employees)
            {
                if (await AllocationExists(employee.Id, leaveTypeId, period))
                    continue;

                allocations.Add(new LeaveAllocations
                {
                    EmployeeId = employee.Id,
                    LeaveTypeId = leaveTypeId,
                    Period = period,
                    NumberOfDays = leaveType.DefaultDays
                });
            }

            await AddRangeAsync(allocations);
        }
    }
}