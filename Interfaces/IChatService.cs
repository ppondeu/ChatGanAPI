using System;
using ChatApi.DTOs;
using ChatApi.Entities;

namespace ChatApi.Interfaces;

public interface IChatService
{
    Task<IEnumerable<Chat>> GetChats();
    Task<IEnumerable<Chat>> GetChats(Guid userId);
    Task<Chat> GetChat(Guid chatId);
    Task<Chat> CreateChat(string connectionId, string roomName, IEnumerable<string> otherMemberIds, Guid creatorId, string? image);
    Task<Message> CreateMessage(string connectionId, string messageText, Guid chatId, Guid senderId);
    Task<IEnumerable<Message>> GetMessages(Guid chatId);
    Task<Chat> JoinChat(Guid chatId, Guid userId);
    Task<Chat> LeaveChat(Guid chatId, Guid userId);

    Task NotifyMessage(Message message);
}
