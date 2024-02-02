using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoAPI.Models
{
    public class Todo
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public bool isCompleted { get; set; }

        [ForeignKey(nameof(Owner))]
        public Guid OwnerId { get; set; }

        public User? Owner { get; set; }
    }
}
