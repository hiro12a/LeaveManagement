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
            model.StartDate = model.StartDate.ToUniversalTime();
            model.EndDate = model.EndDate.ToUniversalTime();

            try
            {
                if(ModelState.IsValid)
                {
                    var isRequestValid = await _leaveRequest.CreateLeaveRequest(model);

                    if(isRequestValid)
                    {
                        return RedirectToAction(nameof(MyLeave));
                    }

                    ModelState.AddModelError(string.Empty, "You do not have enough days left");
                    ModelState.AddModelError(string.Empty, "Numer of Days left: " + model.NumberOfDays);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Creating Leave Request");
                throw;
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
                _logger.LogError(ex, "Error Approving Leave Request");
                throw;
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
                _logger.LogError(ex, "Error Canceling Leave Request");
                throw;
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