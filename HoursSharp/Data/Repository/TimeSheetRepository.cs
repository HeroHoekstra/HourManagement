using HoursSharp.Models;

namespace HoursSharp.Data.Repository;

public class TimeSheetRepository : Repository
{
    private readonly ApplicationDbContext _dbContext;
    
    private readonly SheetDayRepository _dayRepository;
    
    public TimeSheetRepository(ApplicationDbContext dbContext, SheetDayRepository dayRepository) : base(dbContext)
    {
        _dbContext = dbContext;
        _dayRepository = dayRepository;
    }

    // Fetching
    // Dates
    public List<TimeSheet> GetByYear(int year)
    {
        return _dbContext.Set<TimeSheet>()
            .Where(t => t.Date.Year == year)
            .ToList();
    }

    public List<TimeSheet> GetByMonth(int month)
    {
        return _dbContext.Set<TimeSheet>()
            .Where(t => t.Date.Month == month)
            .ToList();
    }

    public List<TimeSheet> GetByYearAndMonth(int year, int month)
    {
        return _dbContext.Set<TimeSheet>()
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .ToList();
    }
    
    // User
    public List<TimeSheet> GetByUser(string userId)
    {
        return _dbContext.Set<TimeSheet>()
            .Where(t => t.UserId == userId)
            .ToList();
    }

    public TimeSheet? GetByUserYearAndMonth(string userId, int year, int month)
    {
        return _dbContext.Set<TimeSheet>()
            .Where(
                t => t.UserId == userId && 
                t.Date.Year == year && 
                t.Date.Month == month
            )
            .ToList()
            .FirstOrDefault();
    }
    
    // Post
    public TimeSheet CreateSheetWithDays(TimeSheet timeSheet)
    {
        TimeSheet createdTs = PostItem(timeSheet);
        
        int amountDays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

        for (int i = 0; i < amountDays; i++)
        {
            SheetDay sheetDay = new SheetDay
            {
                Id = Guid.NewGuid().ToString(),
                Date = createdTs.Date.AddDays(i),
                Hours = 0,
                ExtraHours = 1,
                TimeSheet = createdTs
            };
            
            _dayRepository.PostItem(sheetDay);
        }
        
        return createdTs;
    }
}