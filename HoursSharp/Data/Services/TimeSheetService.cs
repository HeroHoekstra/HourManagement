using HoursSharp.Models;

namespace HoursSharp.Data.Repository;

public class TimeSheetService
{
    private readonly TimeSheetRepository _tsRepository;
    private readonly SheetDayRepository _dayRepository;

    public TimeSheetService(TimeSheetRepository tsRepository, SheetDayRepository dayRepository)
    {
        _tsRepository = tsRepository;
        _dayRepository = dayRepository;
    }
    
    public Dictionary<TimeSheet, List<SheetDay>> GetTimeSheetWithDays(string userId)
    {
        Dictionary<TimeSheet, List<SheetDay>> timeSheets = new Dictionary<TimeSheet, List<SheetDay>>();
        foreach (TimeSheet timeSheet in _tsRepository.GetByUser(userId))
        {
            timeSheets.Add(timeSheet, _dayRepository.GetByTimeSheetId(timeSheet.Id));
        }
        
        return timeSheets;
    }

    public Dictionary<TimeSheet, float> getTimeSheetHours(string userId)
    {
        Dictionary<TimeSheet, List<SheetDay>> timeSheets = GetTimeSheetWithDays(userId);
        
        Dictionary<TimeSheet, float> totalHours = new Dictionary<TimeSheet, float>();
        foreach (KeyValuePair<TimeSheet, List<SheetDay>> timeSheet in timeSheets)
        {
            float hours = 0f;
            foreach (SheetDay sheetDay in timeSheet.Value)
            {
                hours += sheetDay.Hours + sheetDay.ExtraHours;
            }
            
            totalHours.Add(timeSheet.Key, hours);
        }
        
        return totalHours;
    }

    public List<float> getHours(string timeSheetId)
    {
        List<float> hours = new List<float>();
        
        List<SheetDay> sheetDays = _dayRepository.GetByTimeSheetId(timeSheetId);

        float hour = 0f;
        float extraHours = 0f;
        foreach (SheetDay sheetDay in sheetDays)
        {
            hour += sheetDay.Hours;
            extraHours += sheetDay.ExtraHours;
        }
        
        hours.Add(hour);
        hours.Add(extraHours);
        hours.Add(hour + extraHours);

        return hours;
    }
}