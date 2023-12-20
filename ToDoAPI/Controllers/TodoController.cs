using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Data;
using ToDoAPI.Models;
using ToDoAPI.Services;
using ToDoAPI.Services.Interfaces;

namespace ToDoAPI.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService todoService;
        public TodoController(ITodoService todoService)
        {
            this.todoService = todoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetTodos([FromQuery] int page)
        {
            //await Task.Delay(1000);
            var todos = await todoService.GetTodosAsync(page);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodoById(int id)
        {
            var todo = await todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<Todo>> AddTodo(Todo todo)
        {
            await todoService.AddTodoAsync(todo);
            return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, Todo todo)
        {
            try
            {
                var existingTodo = await todoService.GetTodoByIdAsync(id);

                if (existingTodo == null)
                {
                    return NotFound(); // Return 404 Not Found if the Todo with the specified ID is not found
                }

                // Update the existingTodo based on the updatedTodo properties
                existingTodo.Title = todo.Title;
                existingTodo.isCompleted = todo.isCompleted;

                await todoService.UpdateTodoAsync(existingTodo);

                return Ok(existingTodo); // Return 200 OK along with the updated Todo
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine(ex);
                return StatusCode(500, "Internal Server Error"); // Return 500 Internal Server Error for unexpected errors
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            await todoService.DeleteTodoAsync(id);

            return NoContent();
        }
    }
}
