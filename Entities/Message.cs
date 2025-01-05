using System.Text.Json.Serialization;

namespace ChatApi.Entities
{
    public class Message
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        [JsonIgnore]
        public Chat? Chat { get; set; }
        public Guid SenderId { get; set; }
        public User? Sender { get; set; }
        public required string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
