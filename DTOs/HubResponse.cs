using ChatApi.Entities;

namespace ChatApi.DTOs;

public class HubResponse<T>
{
    // public ActivityType ActivityType { get; set; }
    public Guid ChatId { get; set; }
    public T? Response { get; set; }
}
