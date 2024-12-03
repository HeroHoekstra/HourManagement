using Microsoft.EntityFrameworkCore;
using HoursSharp.Models;

namespace HoursSharp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<TimeSheet> TimeSheets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<SheetDay> SheetDays { get; set; }
}