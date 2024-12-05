using HoursSharp.Models;

namespace HoursSharp.Data.Repository;

public class UserRepository : Repository
{
    private readonly ApplicationDbContext _dbContext;
    
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public User? GetByEmail(string email)
    {
        return _dbContext.Set<User>()
            .FirstOrDefault(u => u.Email == email);
    }
}