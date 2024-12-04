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
        return _dbContext.Set<SheetDay>()
            .Where(s => s.TimeSheetId == id)
            .OrderBy(s => s.Date)
            .ToList();
    }
}