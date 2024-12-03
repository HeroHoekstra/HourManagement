using HoursSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace HoursSharp.Data.Repository;

public abstract class Repository
{
    private readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Fetching
    public List<T> GetItems<T>() where T : class
    {
        _dbContext.ChangeTracker.Clear();
        return _dbContext.Set<T>().ToList();
    }
    
    public T? GetById<T>(string id) where T : class
    {
        return _dbContext.Set<T>().Find(id);
    }
    
    // Posting
    public T PostItem<T>(T item) where T : class, IModel
    {
        item.Id = Guid.NewGuid().ToString();
        
        var entityEntry = _dbContext.Add(item);
        
        _dbContext.SaveChanges();
        
        return entityEntry.Entity;
    }
}