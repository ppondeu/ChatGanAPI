using ChatApi.Entities;

namespace ChatApi.Interfaces;

public interface IChatRepository
{
    Task<IEnumerable<Chat>> GetChats();
    Task<IEnumerable<Chat>> GetChats(Guid userId);
    Task<Chat?> GetChat(Guid chatId);
    Task<Chat?> CreateChat(Chat chat);
    Task<Message?> CreateMessage(Message message);
    Task<IEnumerable<Message>> GetMessages(Guid chatId);
    // join chat
    Task<Chat?> JoinChat(User user, Chat chat);
    // leave chat
    Task<Chat?> LeaveChat(User user, Chat chat);
}
