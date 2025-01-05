using System.Text.Json.Serialization;

namespace ChatApi.Entities
{
    public class Chat
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid CreatorId { get; set; }
        public User? Creator { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public ICollection<User>? Members { get; set; }
        public ICollection<Message>? Messages { get; set; }

        public Message? GetLastMessage()
        {
            return Messages?.OrderByDescending(m => m.SentAt).FirstOrDefault();
        }
    }
}
