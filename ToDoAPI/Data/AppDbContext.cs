using Microsoft.EntityFrameworkCore;
using ToDoAPI.Models;

namespace ToDoAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Todo> Todos { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Family> Families { get; set; } = null!;

}
