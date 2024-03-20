using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models.ViewModels;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILogger<LeaveRequestController> _logger;
        private readonly ILeaveRequestRepository _leaveRequest;
        private readonly ILeaveTypeRepository _leaveType;
        private readonly IMapper _mapper;
        public LeaveRequestController(ILogger<LeaveRequestController> logger, IMapper mapper, ILeaveRequestRepository leaveRequestRepository, ILeaveTypeRepository leaveType)
        {
            _logger = logger;
            _leaveRequest = leaveRequestRepository;
            _leaveType = leaveType;
            _mapper = mapper;
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index()
        {
            var model = await _leaveRequest.GetAdminLeaveRequestList();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new LeaveRequestCreateVM
            {
                LeaveTypes = new SelectList(await _leaveType.GetAllAsync(), "Id", "Name")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestCreateVM model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    await _leaveRequest.CreateLeaveRequest(model);
                    return RedirectToAction(nameof(MyLeave));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An Error Has Occured. Please Try Again Later");
            }

            model.LeaveTypes = new SelectList(await _leaveType.GetAllAsync(), "Id", "Name");
            return View(model);       
        }

        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> ApproveRequest(int id, bool approved)
        {  
            try
            {
                await _leaveRequest.ChangeApprovalStatus(id, approved);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An Error Has Occured. Please Try Again Later");
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _leaveRequest.CancelLeaveRequest(id);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An Error Has Occured. Please Try Again Later");
            }
            return RedirectToAction(nameof(MyLeave));
        }

        public async Task<IActionResult> MyLeave()
        {
            var model = await _leaveRequest.GetMyLeaveDetails();
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}