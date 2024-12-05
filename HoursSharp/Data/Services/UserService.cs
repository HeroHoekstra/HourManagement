using System.Text.RegularExpressions;
using HoursSharp.Models;
using Microsoft.AspNetCore.Identity;

namespace HoursSharp.Data.Repository;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool ValidEmail(string email)
    {
        User? existing = _userRepository.GetByEmail(email);

        if (existing != null)
        {
            return false;
        }

        var regex = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        Match match = Regex.Match(email, regex);
        if (!match.Success)
        {
            return false;
        }
        
        return true;
    }

    public bool Login(string email, string password)
    {
        User? user = _userRepository.GetByEmail(email);

        if (user == null)
        {
            return false;
        }

        if (!ConfirmPassword(user.Password, password))
        {
            return false;
        }

        return true;
    }

    public string HashPassword(string password)
    {
        var passHasher = new PasswordHasher<object>();
        
        return passHasher.HashPassword(null, password);
    }

    private bool ConfirmPassword(string password, string plainPassword)
    {
        var passHasher = new PasswordHasher<object>();
        var verifyResult = passHasher.VerifyHashedPassword(null, password, plainPassword);
        
        return verifyResult == PasswordVerificationResult.Success;
    }
}