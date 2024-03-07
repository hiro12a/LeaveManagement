using AutoMapper;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    public class LeaveController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitofWork _unitofwork;
        public LeaveController(IMapper mapper, IUnitofWork unitofWork)
        {
            _unitofwork = unitofWork;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            // Map LeaveTypeVM to LeaveType and display all the data
            // We don't want to display directly from the database so we use LeaveTypeVM
            var leaveTypes = _mapper.Map<List<LeaveTypeVM>>(await _unitofwork.LeaveTypes.GetAllAsync());
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
                var leaveTypes = await _unitofwork.LeaveTypes.GetAsync(id);   
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
                    await _unitofwork.LeaveTypes.AddAsync(obj); 
                }
                else
                {
                    // Update it 
                    await _unitofwork.LeaveTypes.UpdateAsync(obj);
                }
                
                 return RedirectToAction(nameof(Index));
            }

            return View();
        }
        public async Task<IActionResult> Delete(int id)
        {
            await _unitofwork.LeaveTypes.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}