using ChatApi.DTOs;
using ChatApi.Entities;
using ChatApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController(IChatService chatService, IFileService fileService) : ControllerBase
    {
        private readonly IChatService _chatService = chatService;
        private readonly IFileService _fileService = fileService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ChatResponse>>>> GetChats()
        {
            var user = HttpContext.Items["user"] as User ?? throw new UnauthorizedAccessException("Unauthorized");
            var chats = await _chatService.GetChats(user.Id);
            var chatResponses = chats.Select(c => new ChatResponse
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt,
                CreatorId = c.CreatorId,
                Image = c.Image,
                LastMessage = c.GetLastMessage() != null ? new MessageResponse
                {
                    Id = c.GetLastMessage()!.Id,
                    ChatId = c.GetLastMessage()!.ChatId,
                    SenderId = c.GetLastMessage()!.SenderId,
                    Sender = new UserResponse(c.GetLastMessage()!.Sender!),
                    MessageText = c.GetLastMessage()!.MessageText,
                    SentAt = c.GetLastMessage()!.SentAt,
                    DeletedAt = c.GetLastMessage()!.DeletedAt
                } : null,
                Members = c.Members?.Select(m => new UserResponse(m)).ToList(),
                Messages = c.Messages?.Select(m => new MessageResponse
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Sender = new UserResponse(m.Sender!),
                    MessageText = m.MessageText,
                    SentAt = m.SentAt,
                    DeletedAt = m.DeletedAt
                }).ToList()
            });
            return Ok(new ApiResponse<IEnumerable<ChatResponse>>
            {
                StatusCode = 200,
                Errors = null,
                Message = "Chats retrieved",
                Data = chatResponses
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ChatResponse>>> GetChat(Guid id)
        {
            var chat = await _chatService.GetChat(id);
            var chatResponse = new ChatResponse
            {
                Id = chat.Id,
                Name = chat.Name,
                Image = chat.Image,
                CreatedAt = chat.CreatedAt,
                DeletedAt = chat.DeletedAt,
                CreatorId = chat.CreatorId,
                Creator = chat.Creator != null ? new UserResponse(chat.Creator) : null,
                Members = chat.Members?.Select(m => new UserResponse(m)).ToList(),
                Messages = chat.Messages?.Select(m => new MessageResponse
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderId = m.SenderId,
                    Sender = new UserResponse(m.Sender!),
                    MessageText = m.MessageText,
                    SentAt = m.SentAt,
                    DeletedAt = m.DeletedAt
                }).ToList()
            };

            return Ok(new ApiResponse<ChatResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "Chat retrieved successfully",
                Data = chatResponse
            });
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ApiResponse<Chat>>> CreateChat([FromForm] ChatCreateDto chatCreateDto)
        {

            Console.WriteLine(JsonConvert.SerializeObject(chatCreateDto));
            if (HttpContext.Items["user"] is not User user)
            {
                return Unauthorized(new ApiResponse<Chat>
                {
                    StatusCode = 401,
                    Errors = ["Unauthorized"],
                    Message = "User not found",
                    Data = null
                });
            }

            string? fileName = null;
            if (chatCreateDto.Image != null)
            {
                fileName = await _fileService.ValidateAndSaveFileAsync(chatCreateDto.Image, "Uploads");
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(JsonConvert.SerializeObject(chatCreateDto));
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            var chat = await _chatService.CreateChat(chatCreateDto.ConnectionId, chatCreateDto.Name, chatCreateDto.OtherMemberIds, user.Id, fileName);

            return Created($"api/chats/{chat.Id}", new ApiResponse<Chat>
            {
                StatusCode = 201,
                Errors = null,
                Message = "Chat created",
                Data = chat
            });
        }

        [HttpPost("{id}/message")]
        public async Task<ActionResult<ApiResponse<Message>>> CreateMessage(Guid id, [FromBody] MessageCreateDto messageCreateDto)
        {
            if (HttpContext.Items["user"] is not User user)
            {
                return Unauthorized(new ApiResponse<Chat>
                {
                    StatusCode = 401,
                    Errors = ["Unauthorized"],
                    Message = "User not found",
                    Data = null
                });
            }
            var message = await _chatService.CreateMessage(messageCreateDto.ConnectionId, messageCreateDto.MessageText, id, user.Id);
            return Created($"api/chats/{id}/message/{message.Id}", new ApiResponse<Message>
            {
                StatusCode = 201,
                Errors = null,
                Message = "Message created",
                Data = message
            });
        }

        [HttpPost("{id}/join")]
        [Consumes("application/json")]
        public async Task<ActionResult<ApiResponse<Chat>>> JoinChat(Guid id)
        {
            if (HttpContext.Items["user"] is not User user)
            {
                return Unauthorized(new ApiResponse<Chat>
                {
                    StatusCode = 401,
                    Errors = ["Unauthorized"],
                    Message = "User not found",
                    Data = null
                });
            }
            var chat = await _chatService.JoinChat(id, user.Id);
            return Ok(new ApiResponse<Chat>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User joined chat",
                Data = chat
            });
        }

        [HttpPost("{id}/leave")]
        [Consumes("application/json")]
        public async Task<ActionResult<ApiResponse<Chat>>> LeaveChat(Guid id)
        {
            if (HttpContext.Items["user"] is not User user)
            {
                return Unauthorized(new ApiResponse<Chat>
                {
                    StatusCode = 401,
                    Errors = ["Unauthorized"],
                    Message = "User not found",
                    Data = null
                });
            }
            var chat = await _chatService.LeaveChat(id, user.Id);
            return Ok(new ApiResponse<Chat>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User left chat",
                Data = chat
            });
        }
    }
}
