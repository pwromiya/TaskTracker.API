namespace TaskTracker.Contracts.Requests;
public class CreateTaskRequest
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int ProjectId { get; set; }
    public int Status { get; set; } = 0;
}
