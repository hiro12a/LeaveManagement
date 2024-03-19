using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;
using LeaveManagement.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace LeaveManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager; // Logs in the user
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ILeaveAllocationRepository _leaveAllocation;
        private readonly ILeaveTypeRepository _leaveType;

        public EmployeeController(UserManager<Employee> userManager, 
        RoleManager<IdentityRole> roleManager, 
        SignInManager<Employee> signInManager, 
        IMapper mapper,
        ILeaveAllocationRepository leaveAllocationRepository, 
        ILeaveTypeRepository leaveType)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _leaveAllocation = leaveAllocationRepository;
            _leaveType = leaveType;
        }

        // Login Page
        public IActionResult Index(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // Populate if not empty

            LoginVM loginVM = new()
            {
                RedirectUrl = returnUrl
            };
            
            return View(loginVM);
        }

        // Log in the user
        [HttpPost]
        public async Task<IActionResult> Index(LoginVM obj)
        {
            if(ModelState.IsValid)
            {
                // Get email and password 
                // Check if they clicked remember me
                var result = await _signInManager.
                PasswordSignInAsync(obj.Email, obj.Password, obj.RememberMe, lockoutOnFailure: false);

                if(result.Succeeded)
                {
                    // Redirect user after they sign in
                    if(string.IsNullOrEmpty(obj.RedirectUrl))
                    {
                        // Go to homepage
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        // Local redirect will redirect to same domain, don't use RedirectToPage
                        return LocalRedirect(obj.RedirectUrl);
                    }
                } 
                else
                {
                    ModelState.AddModelError("","Invalid Login Attempt");
                }
            }
            return View(obj);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }

        public async Task<IActionResult> Register()
        {
            // Create role if it doesn't exist
            if(!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                // Wait() is the same as getwaiter().getresult()
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).Wait(); 
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).Wait();
            }

            RegisterVM registerVM = new()
            {
                // Dropdownlist for roles
                RoleList = _roleManager.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name, 
                    Value = u.Name
                })
            };

            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM obj)
        {
            // Register the user and connect the employee table to it
            Employee employee = new()
            {
                FirstName = obj.FirstName,
                LastName = obj.LastName, 
                Email = obj.Email, 
                PhoneNumber = obj.PhoneNumber,
                NormalizedEmail = obj.Email.ToUpper(), 
                EmailConfirmed = true,
                JoinDate = obj.JoinDate, 
                DOB = obj.DOB,
                UserName = obj.Email, 
            };

            // Register the employee
            var createEmployee = await _userManager.CreateAsync(employee, obj.Password);
            if(createEmployee.Succeeded)
            {
                // Assign a role to employee if selected
                // Only admins can assign role
                if(!string.IsNullOrEmpty(obj.Role))
                {
                    await _userManager.AddToRoleAsync(employee, obj.Role);
                }
                else
                {
                    // Assigns the employee role by default if no role is selected
                    await _userManager.AddToRoleAsync(employee, SD.Role_Employee);
                }

                // Sign in the employee automatically
                await _signInManager.SignInAsync(employee, isPersistent: false); 

                // Redirect the employee to homepage after they are signed in
                if(string.IsNullOrEmpty(obj.RedirectUrl))
                {
                    // Go home
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Refresh the page
                    // Make sure to use local redirect so it'll only redirect in the same domain
                    return LocalRedirect(obj.RedirectUrl); 
                }
            }

            // If the result is not successful
            foreach(var error in createEmployee.Errors)
            {
                // Returns the error
                ModelState.AddModelError("", error.Description);
            }

            return View(obj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = SD.Role_Admin)]
        // Get the list of employees and their leave allocations
        public async Task<IActionResult> EmployeeLeaveAllocation()
        {
            // Get employees with employee role
            var employee = await _userManager.GetUsersInRoleAsync(SD.Role_Employee);
            var model = _mapper.Map<List<EmployeeListVM>>(employee);
            return View(model);
        }

        [Authorize(Roles = SD.Role_Admin)]
        // Get a detailed view of the employees leave allocation
        public async Task<IActionResult> ViewAllocation(string id)
        {
            var employee = await _leaveAllocation.EmployeeAllocationVM(id);
            return View(employee);
        }

        [Authorize(Roles = SD.Role_Admin)]
        // Edit the employee leave allocation
        public async Task<IActionResult> EditLeaveAllocation(int id)
        {
            var model = await _leaveAllocation.GetEmployeeAllocation(id);
            if(model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> EditLeaveAllocation(int id, EditLeaveAllocationsVM model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var leaveAllocation = await _leaveAllocation.GetAsync(model.Id);
                    if(leaveAllocation == null)
                    {
                        return NotFound();
                    }

                    // Update these from the model
                    leaveAllocation.Period = model.Period;
                    leaveAllocation.NumberOfDays = model.NumberOfDays;

                    // Commit  and save
                    await _leaveAllocation.UpdateAsync(leaveAllocation);

                    return RedirectToAction(nameof(ViewAllocation), new { id = model.EmployeeId });
                }        
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An Error Has Occured. Please Try Again Later");
            }

            model.Employee = _mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(model.EmployeeId));  
            model.LeaveType = _mapper.Map<LeaveTypeVM>(await _leaveType.GetAsync(model.LeaveTypeId));
            return View(model);
        }
    }
}