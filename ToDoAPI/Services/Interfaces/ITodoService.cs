using ToDoAPI.DTOs;
using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces;

public interface ITodoService
{
    Task<List<TodoDto>> GetTodosAsync(int pageToShow, string ownerEmail);
    Task<Todo> GetTodoByIdAsync(int id);
    Task AddTodoAsync(Todo todo);
    Task UpdateTodoAsync(Todo todo);
    Task DeleteTodoAsync(int id);
    Task<int> GetTodosCount(string email);
}
