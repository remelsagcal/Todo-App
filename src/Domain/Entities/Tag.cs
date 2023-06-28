namespace Todo_App.Domain.Entities;

public class Tag : BaseAuditableEntity
{
    public string Title { get; set; } = null!;

    public virtual ICollection<TodoItem> Items { get; set; } = new List<TodoItem>();
}
