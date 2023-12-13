using Microsoft.EntityFrameworkCore;
using ToDoAPI.Models;

namespace ToDoAPI.Data;

public static class TodoSeeder
{
    public static void Seed(IServiceProvider serviceProvider)
    {
        using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
        {
            context.Database.EnsureCreated();

            if (!context.Todos.Any())
            {
                context.Todos.AddRange(
                    new Todo { Title = "Buy groceries", isCompleted = false },
                    new Todo { Title = "Write code", isCompleted = false }
                    );
                context.SaveChanges();
            }
        }
    }
}
