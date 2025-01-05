using ChatApi.Data;
using ChatApi.Entities;
using ChatApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Repositories;

public class ChatRepository(ApplicationDbContext context) : IChatRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Chat>> GetChats()
    {
        return await _context.Chats.Include(c => c.Members).Include(c => c.Messages).ToListAsync();
    }

    public async Task<IEnumerable<Chat>> GetChats(Guid userId)
    {
        return await _context.Chats.Include(c => c.Members).Include(c => c.Messages).Where(c => c.Members!.Any(m => m.Id == userId)).ToListAsync();
    }

    public async Task<Chat?> GetChat(Guid chatId)
    {
        return await _context.Chats.Include(c => c.Members).Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == chatId);
    }
    public async Task<Chat?> CreateChat(Chat chat)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();
        return chat;
    }

    public async Task<Message?> CreateMessage(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IEnumerable<Message>> GetMessages(Guid chatId)
    {
        return await _context.Messages.Where(m => m.ChatId == chatId).ToListAsync();
    }

    public async Task<Chat?> JoinChat(User user, Chat chat)
    {
        chat.Members!.Add(user);
        await _context.SaveChangesAsync();
        return chat;
    }

    public async Task<Chat?> LeaveChat(User user, Chat chat)
    {
        chat.Members!.Remove(user);
        await _context.SaveChangesAsync();
        return chat;
    }
}
