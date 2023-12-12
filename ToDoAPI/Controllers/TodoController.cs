using Microsoft.AspNetCore.Mvc;

namespace ToDoAPI.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
