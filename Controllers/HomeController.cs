using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LeaveManagement.Models;
using Microsoft.AspNetCore.Diagnostics;
using LeaveManagement.Utility;

namespace LeaveManagement.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return RedirectToAction("MyLeave", "LeaveRequest");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Change Error from default to this to make it more readable 
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>(); // Gets unhandled exception

        if(exceptionHandlerPathFeature != null)
        {
            Exception exception = exceptionHandlerPathFeature.Error;
            _logger.LogError(exception, $"Error Encountered By User: {this.User?.Identity?.Name} | Request Id: {requestId}");
        }
        return View(new ErrorViewModel {RequestId = requestId});
    }
}
