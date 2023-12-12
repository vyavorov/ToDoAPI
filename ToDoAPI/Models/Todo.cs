using System.ComponentModel.DataAnnotations;

namespace ToDoAPI.Models
{
    public class Todo
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public bool isCompleted { get; set; }
    }
}
