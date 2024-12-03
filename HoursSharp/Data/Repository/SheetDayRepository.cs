using HoursSharp.Models;

namespace HoursSharp.Data.Repository;

public class SheetDayRepository : Repository
{
    private readonly ApplicationDbContext _dbContext;
    
    public SheetDayRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
    
    // Fetching
    public List<SheetDay> GetByTimeSheetId(string id)
    {
        return _dbContext.SheetDays.Where(d => d.TimeSheetId == id).ToList();
    }
}