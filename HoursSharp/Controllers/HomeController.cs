using System.Diagnostics;
using HoursSharp.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using HoursSharp.Models;

namespace HoursSharp.Controllers;

public class HomeController : Controller
{
    private readonly TimeSheetRepository _tsRepository;
    private readonly SheetDayRepository _dayRepository;
    
    private readonly TimeSheetService _tsService;

    public HomeController(TimeSheetRepository tsRepository, SheetDayRepository dayRepository, TimeSheetService tsService)
    {
        _tsRepository = tsRepository;
        _dayRepository = dayRepository;
        
        _tsService = tsService;
    }
    
    public IActionResult Index()
    {
        string? loggedIn = Request.Cookies["LoggedIn"];
        if (loggedIn == null)
        {
            return RedirectToAction("LoginPage", "User");
        }

        ViewData["timeSheets"] = _tsService.GetTimeSheetWithDays(loggedIn);
        ViewData["totalHours"] = _tsService.getTimeSheetHours(loggedIn);
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}