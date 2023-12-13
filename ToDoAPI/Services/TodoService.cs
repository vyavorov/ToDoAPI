using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.Models;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext dbContext;
    public TodoService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddTodoAsync(Todo todo)
    {
        await dbContext.Todos.AddAsync(todo);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteTodoAsync(int id)
    {
        var todo = await dbContext.Todos.FindAsync(id);
        if (todo != null)
        {
            dbContext.Todos.Remove(todo);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<Todo> GetTodoByIdAsync(int id)
    {
        var todo = await dbContext.Todos.FindAsync(id);

        if (todo == null)
        {
            // Handle the case where the entity with the specified id is not found
            throw new NullReferenceException($"Todo with id {id} not found");
        }

        return todo;
    }

    public async Task<List<Todo>> GetTodosAsync()
    {
        return await dbContext.Todos.ToListAsync();
    }

    public async Task UpdateTodoAsync(Todo todo)
    {
        dbContext.Entry(todo).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }
}
