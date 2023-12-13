﻿using ToDoAPI.Models;

namespace ToDoAPI.Services.Interfaces;

public interface ITodoService
{
    Task<List<Todo>> GetTodosAsync();
    Task<Todo> GetTodoByIdAsync(int id);
    Task AddTodoAsync(Todo todo);
    Task UpdateTodoAsync(Todo todo);
    Task DeleteTodoAsync(int id);
}