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
    
    [HttpGet("/hour")]
    public IActionResult Index()
    {
        ViewData["currentMonth"] = DateTime.Now.ToString("MM");
        ViewData["currentYear"] = DateTime.Now.Year;

        if (TempData["errorMessage"] != null)
        {
            ViewData["errorMessage"] = TempData["errorMessage"];
        }
        
        
        // TODO: The user part is a test and should be removed!
        User user = _userRepository.GetById<User>("0");
        if (user != null)
        {
            TimeSheet? timeSheet = _tsRepository.GetByUserYearAndMonth(user.Id, DateTime.Now.Year, DateTime.Now.Month);
            List<SheetDay> sheetDays = _dayRepository.GetByTimeSheetId(timeSheet.Id);

            ViewData["timeSheet"] = timeSheet;
            ViewData["sheetDays"] = sheetDays;
        }
        
        return View();
    }

    [HttpPost("/api/hour")]
    public IActionResult Create()
    {
        // Check if there already is a sheet for this month
        int month = DateTime.Today.Month;
        int year = DateTime.Today.Year;
        
        List<TimeSheet> timesheets = _tsRepository.GetByYearAndMonth(year, month);

        if (timesheets.Count != 0)
        {
            TempData["errorMessage"] = "Er bestaat al een uren brief voor deze maand";

            return RedirectToAction("Index");
        }
        
        // TODO: The user part is a test and should be removed!
        User user = _userRepository.GetById<User>("0");
        if (user == null)
        {
            TempData["errorMessage"] = "Deze user bestaat niet";
            return RedirectToAction("Index");
        }
        
        _tsRepository.CreateSheetWithDays(new TimeSheet
        {
            Date = new DateTime(year, month, 1),
            User = user
        });
        
        return RedirectToAction("Index");
    }

    [HttpPost("api/hour/update")]
    public IActionResult Update([FromForm] SheetDay[] sheetDays)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        foreach (SheetDay sheetDay in sheetDays)
        {
            _userRepository.PutItem(sheetDay);
        }

        return RedirectToAction("Index", "Home");
    }
}