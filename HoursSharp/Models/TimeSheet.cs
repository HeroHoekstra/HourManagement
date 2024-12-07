namespace HoursSharp.Models;

public class TimeSheet : IModel
{
    public string Id { get; set; } // Primary Key
    
    public DateTime Date { get; set; }
    
    public ICollection<SheetDay> Days { get; set; }

    public string UserId { get; set; }
    public User User { get; set; }

    public void PrintName()
    {
        Console.WriteLine(User.FullName());
    }
}