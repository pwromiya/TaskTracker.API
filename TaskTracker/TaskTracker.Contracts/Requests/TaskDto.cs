namespace TaskTracker.Contracts.Requests;

// Task data transfer object.
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public string StatusText => Status switch // Readable status text obtained from the Status object
    {
        0 => "Todo",
        1 => "InProgress",
        2 => "Review",
        3 => "Blocked",
        4 => "Done",
        5 => "Cancelled"
    };

}
