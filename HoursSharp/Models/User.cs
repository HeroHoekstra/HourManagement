namespace HoursSharp.Models;

public class User : IModel
{
    public string Id { get; set; }
    
    public string FirstName { get; set; }
    public string? Infix { get; set; }
    public string LastName { get; set; }

    public string FullName()
    {
        return string.IsNullOrEmpty(Infix) ? $"{FirstName} {LastName}" : $"{FirstName} {Infix} {LastName}";
    }
    
    public string Email { get; set; }
    
    public ICollection<TimeSheet> TimeSheets { get; set; }
}