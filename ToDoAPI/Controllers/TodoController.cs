using Microsoft.AspNetCore.Mvc;
using ToDoAPI.Data;
using ToDoAPI.Models;
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
        public async Task<ActionResult<List<Todo>>> GetTodos()
        {
            var todos = await todoService.GetTodosAsync();
            return Ok(todos);
        }
    }
}
