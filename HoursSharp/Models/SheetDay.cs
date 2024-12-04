namespace HoursSharp.Models;

public class SheetDay : IModel
{
    public string Id { get; set; } // Primary Key
    
    public DateTime Date { get; set; }
    
    public float Hours { get; set; }
    public float ExtraHours { get; set; }
    
    public string? Description { get; set; }
    
    public string TimeSheetId { get; set; }
    public TimeSheet TimeSheet { get; set; }
}