using ChatApi.Common.Exceptions;
using ChatApi.DTOs;
using ChatApi.Entities;
using ChatApi.Hubs;
using ChatApi.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace ChatApi.Services;

public class ChatService(IChatRepository chatRepository, IUserRepository userRepository, IHubContext<ChatHub> chatHub) : IChatService
{
    private readonly IChatRepository _chatRepository = chatRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHubContext<ChatHub> _chatHub = chatHub;

    public async Task<IEnumerable<Chat>> GetChats()
    {
        return await _chatRepository.GetChats();
    }

    public async Task<IEnumerable<Chat>> GetChats(Guid userId)
    {
        return await _chatRepository.GetChats(userId);
    }

    public async Task<Chat> GetChat(Guid chatId)
    {
        return await _chatRepository.GetChat(chatId) ?? throw new NotFoundError("Chat not found");
    }

    public async Task<Chat> CreateChat(string connectionId, string roomName, IEnumerable<string> otherMemberIds, Guid creatorId, string? image)
    {
        var members = new List<User>();
        var creator = await _userRepository.GetUserById(creatorId) ?? throw new NotFoundError("User not found");
        members.Add(creator);
        foreach (var memberId in otherMemberIds)
        {
            var member = await _userRepository.GetUserById(Guid.Parse(memberId)) ?? throw new NotFoundError("User not found");
            members.Add(member);
        }
        var chat = new Chat
        {
            Name = roomName,
            CreatorId = creatorId,
            Image = image,
            Members = members
        };
        var chatCreated = await _chatRepository.CreateChat(chat) ?? throw new InternalServerError("Failed to create chat");
        await AddUserToGroup(chatCreated.Id, connectionId);
        await NotifyNotification(chatCreated.Id, $"You have been created a chat: {chatCreated.Name}");
        return chatCreated;
    }

    public async Task<Message> CreateMessage(string connectionId, string messageText, Guid chatId, Guid senderId)
    {
        var chat = await _chatRepository.GetChat(chatId) ?? throw new NotFoundError("Chat not found");
        if (!chat.Members!.Any(m => m.Id == senderId))
        {
            throw new BadRequestError("User not in chat");
        }
        var message = new Message
        {
            ChatId = chatId,
            SenderId = senderId,
            MessageText = messageText,
            SentAt = DateTime.Now,
        };
        var newMessage = await _chatRepository.CreateMessage(message) ?? throw new InternalServerError("Failed to create message");
        await NotifyMessage(newMessage);
        return newMessage;
    }

    public async Task<IEnumerable<Message>> GetMessages(Guid chatId)
    {
        return await _chatRepository.GetMessages(chatId);
    }

    public async Task<Chat> JoinChat(Guid chatId, Guid userId)
    {
        var existingChat = await _chatRepository.GetChat(chatId) ?? throw new NotFoundError("Chat not found");
        var user = await _userRepository.GetUserById(userId) ?? throw new NotFoundError("User not found");
        if (existingChat.Members!.Any(m => m.Id == userId))
        {
            throw new BadRequestError("User already in chat");
        }
        var chat = await _chatRepository.JoinChat(user, existingChat) ?? throw new InternalServerError("Failed to join chat");
        await AddUserToGroup(chatId, userId.ToString());
        await NotifyNotification(chatId, $"User {user.Username} has joined the chat");
        return chat;
    }
    public async Task<Chat> LeaveChat(Guid chatId, Guid userId)
    {
        var existingChat = await _chatRepository.GetChat(chatId) ?? throw new NotFoundError("Chat not found");
        var user = await _userRepository.GetUserById(userId) ?? throw new NotFoundError("User not found");
        if (!existingChat.Members!.Any(m => m.Id == userId))
        {
            throw new BadRequestError("User not in chat");
        }
        var chat = await _chatRepository.LeaveChat(user, existingChat) ?? throw new InternalServerError("Failed to leave chat");
        await RemoveUserFromGroup(chatId, userId.ToString());
        await NotifyNotification(chatId, $"User {user.Username} has left the chat");
        return chat;
    }

    public async Task NotifyMessage(Message message)
    {
        var response = new HubResponse<Message>
        {
            ChatId = message.ChatId,
            Response = message
        };
        await _chatHub.Clients.Group(message.ChatId.ToString()).SendAsync("ReceiveMessage", response);
    }

    public async Task NotifyNotification(Guid chatId, string message)
    {
        var response = new HubResponse<string>
        {
            ChatId = chatId,
            Response = message
        };
        await _chatHub.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", response);
    }

    public async Task AddUserToGroup(Guid chatId, string connectionId)
    {
        await _chatHub.Groups.AddToGroupAsync(connectionId, chatId.ToString());
    }

    public async Task RemoveUserFromGroup(Guid chatId, string connectionId)
    {
        await _chatHub.Groups.RemoveFromGroupAsync(connectionId, chatId.ToString());
    }
}
