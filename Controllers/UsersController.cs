using ChatApi.Common.Exceptions;
using ChatApi.DTOs;
using ChatApi.Entities;
using ChatApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetUsers()
        {
            var users = await _userService.GetUsers();
            var userResponses = users.Select(user => new UserResponse(user));
            return Ok(new ApiResponse<IEnumerable<UserResponse>>
            {
                StatusCode = 200,
                Errors = null,
                Message = "Users retrieved",
                Data = userResponses
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserResponse>>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            return Ok(new ApiResponse<UserResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User retrieved",
                Data = new UserResponse(user)
            });
        }

        [HttpGet("friends")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetFriends()
        {
            var user = HttpContext.Items["user"] as User ?? throw new UnauthorizedError("User not found");
            var friends = await _userService.GetFriends(user.Id);
            var friendResponses = friends.Select(friend => new UserResponse(friend));
            return Ok(new ApiResponse<IEnumerable<UserResponse>>
            {
                StatusCode = 200,
                Errors = null,
                Message = "Friends retrieved",
                Data = friendResponses
            });
        }

        [HttpPatch]
        public async Task<ActionResult<ApiResponse<UserResponse>>> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
        {
            var user = HttpContext.Items["user"] as User ?? throw new UnauthorizedError("User not found");
            var userUpdated = await _userService.UpdateUser(user.Id, userUpdateDto);

            return Ok(new ApiResponse<UserResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User updated",
                Data = new UserResponse(userUpdated)
            });
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<UserResponse>>> DeleteUser(Guid id)
        {
            var user = HttpContext.Items["user"] as User ?? throw new UnauthorizedError("User not found");
            await _userService.DeleteUser(user.Id);
            return Ok(new ApiResponse<UserResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User deleted",
                Data = new UserResponse(user)
            });
        }

        [HttpGet("me")]
        public ActionResult<ApiResponse<UserResponse>> GetMe()
        {
            var user = Request.HttpContext.Items["user"] as User ?? throw new UnauthorizedError("User not found");
            return Ok(new ApiResponse<UserResponse>
            {
                StatusCode = 200,
                Errors = null,
                Message = "User retrieved",
                Data = new UserResponse(user)
            });
        }

    }
}
