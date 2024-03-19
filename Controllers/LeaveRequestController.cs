using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models.ViewModels;
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
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An Error Has Occured. Please Try Again Later");
            }

            model.LeaveTypes = new SelectList(await _leaveType.GetAllAsync(), "Id", "Name");
            return View(model);       
        }

        public async Task<IActionResult> MyLeave()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}