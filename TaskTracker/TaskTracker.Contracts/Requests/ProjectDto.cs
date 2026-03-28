namespace TaskTracker.Contracts.Requests;

// Project data transfer object
public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }

}
