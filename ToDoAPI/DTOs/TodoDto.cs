namespace ToDoAPI.DTOs;

public class TodoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
}
