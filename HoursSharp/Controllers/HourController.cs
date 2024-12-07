using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HoursSharp.Data.Repository;
using HoursSharp.Models;

namespace HoursSharp.Controllers;

public class HourController : Controller
{
    private readonly TimeSheetRepository _tsRepository;
    private readonly SheetDayRepository _dayRepository;
    private readonly UserRepository _userRepository;

    public HourController
    (
        TimeSheetRepository tsRepository, 
        SheetDayRepository dayRepository,
        UserRepository userRepository
    )
    {
        _tsRepository = tsRepository;
        _dayRepository = dayRepository;
        _userRepository = userRepository;
    }
    
    [HttpGet("/timesheet")]
    public IActionResult Index()
    {
        string? userId = Request.Cookies["LoggedIn"];
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        User user = _userRepository.GetById<User>(userId);
        if (user != null)
        {
            TimeSheet? timeSheet = _tsRepository.GetByUserYearAndMonth(user.Id, DateTime.Now.Year, DateTime.Now.Month);
            List<SheetDay> sheetDays = _dayRepository.GetByTimeSheetId(timeSheet.Id);

            ViewData["timeSheet"] = timeSheet;
            ViewData["sheetDays"] = sheetDays;
        }
        
        ViewData["month"] = DateTime.Now.ToString("MM");
        ViewData["year"] = DateTime.Now.Year;
        ViewData["day"] = DateTime.Now.Day - 1;
        if (TempData["errorMessage"] != null)
        {
            ViewData["errorMessage"] = TempData["errorMessage"];
        }
        
        return View();
    }

    [HttpGet("/timesheet/{id}")]
    public IActionResult TimeSheetById(string id)
    {
        TimeSheet? timeSheet = _tsRepository.GetById<TimeSheet>(id);
        if (timeSheet == null)
        {
            return NotFound();
        }
        List<SheetDay> sheetDays = _dayRepository.GetByTimeSheetId(timeSheet.Id);
        
        ViewData["timeSheet"] = timeSheet;
        ViewData["sheetDays"] = sheetDays;
        
        ViewData["month"] = timeSheet.Date.Month;
        ViewData["year"] = timeSheet.Date.Year;
        
        return View("Index");
    }
    
    [HttpPost("/api/timesheet")]
    public IActionResult Create()
    {
        // Check if User exists
        string? userId = Request.Cookies["LoggedIn"];
        if (userId == null)
        {
            return RedirectToAction("Login", "User");
        }
        
        User user = _userRepository.GetById<User>(userId);
        if (user == null)
        {
            TempData["errorMessage"] = "Deze user bestaat niet";
            return RedirectToAction("Index");
        }
        
        // Check if there already is a sheet for this month
        int month = DateTime.Today.Month;
        int year = DateTime.Today.Year;
        
        TimeSheet? timesheets = _tsRepository.GetByUserYearAndMonth(user.Id, year, month);

        if (timesheets != null)
        {
            TempData["errorMessage"] = "Er bestaat al een uren brief voor deze maand";
            return RedirectToAction("Index");
        }
        
        // Create TimeSheet
        _tsRepository.CreateSheetWithDays(new TimeSheet
        {
            Date = new DateTime(year, month, 1),
            User = user
        });
        
        return RedirectToAction("Index");
    }

    [HttpPost("api/timesheet/update")]
    public IActionResult Update([FromForm] SheetDay[] sheetDays)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        foreach (SheetDay sheetDay in sheetDays)
        {
            _dayRepository.PutItem(sheetDay);
        }

        return RedirectToAction("Index", "Home");
    }
}