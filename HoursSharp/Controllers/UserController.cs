using HoursSharp.Data.Repository;
using HoursSharp.Models;
using Microsoft.AspNetCore.Mvc;

namespace HoursSharp.Controllers;

public class UserController : Controller
{
    private readonly UserRepository _userRepository;
    
    private readonly UserService _userService;

    public UserController(UserRepository userRepository, UserService userService)
    {
        _userRepository = userRepository;
        
        _userService = userService;
    }
    
    [HttpGet("/user")]
    public IActionResult Index()
    {
        ViewData["users"] = _userRepository.GetItems<User>();
        
        return View();
    }

    // Creation
    [HttpGet("/user/create")]
    public IActionResult CreatePage()
    {
        return View("Create");
    }

    [HttpPost("/api/user/create")]
    public IActionResult Create([FromForm] User user)
    {
        if (!_userService.ValidEmail(user.Email))
        {
            TempData["errorMessage"] = "Email is incorrect of bestaat al";
            
            return RedirectToAction("CreatePage");
        }
        
        user.Password = _userService.HashPassword(user.Password);
        
        _userRepository.PostItem(user);
        
        return RedirectToAction("Index");
    }

    [HttpGet("login")]
    public IActionResult LoginPage()
    {
        if (TempData["errorMessage"] == null)
        {
            ViewData["errorMessage"] = TempData["errorMessage"];
        }
        
        return View("Login");
    }

    [HttpPost("/api/login")]
    public IActionResult Login([FromForm] string Email, [FromForm] string Password)
    {
        bool success = _userService.Login(Email, Password);

        if (!success)
        {
            TempData["errorMessage"] = "Invalid email or password";
            
            return View("Login");
        }

        string userId = _userRepository.GetByEmail(Email).Id;
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(10),
        };
        Response.Cookies.Append("LoggedIn", userId, cookieOptions);
        
        return RedirectToAction("Index", "Home");
    }
}