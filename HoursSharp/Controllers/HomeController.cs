using System.Diagnostics;
using HoursSharp.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using HoursSharp.Models;

namespace HoursSharp.Controllers;

public class HomeController : Controller
{
    private readonly TimeSheetRepository _tsRepository;

    public HomeController(TimeSheetRepository tsRepository)
    {
        _tsRepository = tsRepository;
    }
    
    public IActionResult Index()
    {
        string? loggedIn = Request.Cookies["LoggedIn"];
        if (loggedIn == null)
        {
            return RedirectToAction("Login", "User");
        }

        List<TimeSheet> timeSheets = _tsRepository.GetByUser(loggedIn);
        ViewData["timeSheets"] = timeSheets;
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}