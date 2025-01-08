using ChatApi.Entities;
using ChatApi.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ChatApi.Hubs;

public class ChatHub(IAuthService authService, IChatService chatService, IDistributedCache cache) : Hub
{
    private readonly IAuthService _authService = authService;
    private readonly IChatService _chatService = chatService;
    private readonly IDistributedCache _cache = cache;
    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var accessToken = httpContext?.Request.Cookies["accessToken"];
        // decode jwt
        try
        {
            var user = _authService.ValidateToken(accessToken!, true).Result;
            Console.WriteLine(JsonConvert.SerializeObject(user));
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"Connection ID: {connectionId}");
            var chats = _chatService.GetChats(user.Id).Result;
            var cacheKey = $"user::{user.Id}::connection";
            foreach (var chat in chats)
            {
                Groups.AddToGroupAsync(connectionId, chat.Id.ToString());
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Unauthorized");
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var httpContext = Context.GetHttpContext();
        var accessToken = httpContext?.Request.Cookies["accessToken"];
        // decode jwt
        try
        {
            var user = _authService.ValidateToken(accessToken!, true).Result;
            Console.WriteLine(JsonConvert.SerializeObject(user));
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"Connection ID: {connectionId}");
            var chats = _chatService.GetChats(user.Id).Result;
            foreach (var chat in chats)
            {
                Groups.RemoveFromGroupAsync(connectionId, chat.Id.ToString());
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Unauthorized");
        }
        return base.OnDisconnectedAsync(exception);
    }
}
