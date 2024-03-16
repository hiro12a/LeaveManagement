using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class LeaveController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILeaveAllocationRepository _leaveAlloaction;
        private readonly ILeaveTypeRepository _leaveType;
        public LeaveController(IMapper mapper, 
                    ILeaveAllocationRepository leaveAllocationRepository,
                    ILeaveTypeRepository leaveTypRepository)
        {
            _mapper = mapper;
            _leaveAlloaction = leaveAllocationRepository;
            _leaveType = leaveTypRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Map LeaveTypeVM to LeaveType and display all the data
            // We don't want to display directly from the database so we use LeaveTypeVM
            var leaveTypes = _mapper.Map<List<LeaveTypeVM>>(await _leaveType.GetAllAsync());
            return View(leaveTypes);
        }
        public async Task<IActionResult> Upsert(int? id)
        {
            // If there is no existing object, go to a create view
            if(id == null || id == 0)
            {
                return View(new LeaveTypeVM());
            }
            else
            {
                // There is an existing obj, get the data and display it
                var leaveTypes = await _leaveType.GetAsync(id);   
                if(leaveTypes == null)
                {
                    return NotFound();
                }         

                var leaveTypeVM = _mapper.Map<LeaveTypeVM>(leaveTypes);
                return View(leaveTypeVM);                
            }
        }
        [HttpPost]
        public async Task<IActionResult> Upsert(LeaveType obj)
        {
            // Make sure modelstate is valid first
            if(ModelState.IsValid)
            {
                if(obj.Id == 0)
                {
                    // Create the obj 
                    await _leaveType.AddAsync(obj); 
                }
                else
                {
                    // Update it 
                    await _leaveType.UpdateAsync(obj);
                }
                
                 return RedirectToAction(nameof(Index));
            }

            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _leaveType.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateLeave(int id)
        {
            await _leaveAlloaction.LeaveAllocation(id);
            return RedirectToAction(nameof(Index));
        }
    }
}