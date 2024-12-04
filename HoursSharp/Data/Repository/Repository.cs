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
    
    // Update
    public T? PutItem<T>(T item) where T : class, IModel
    {
        T? oldItem = GetById<T>(item.Id);
        if (oldItem == null)
        {
            return null;
        }

        _dbContext.Entry(oldItem).CurrentValues.SetValues(item);
        
        _dbContext.Entry(oldItem).State = EntityState.Modified;
        
        _dbContext.SaveChanges();
        
        return oldItem;
    }
}