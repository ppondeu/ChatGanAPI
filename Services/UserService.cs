using ChatApi.Common.Exceptions;
using ChatApi.DTOs;
using ChatApi.Entities;
using ChatApi.Interfaces;

namespace ChatApi.Services;

public class UserService(IUserRepository userRepository, IPasswordHasher passwordHasher) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<User> CreateUser(UserCreateDto userCreateDto)
    {
        var existingUser = await _userRepository.GetUserByEmail(userCreateDto.Email);
        if (existingUser != null)
        {
            throw new BadRequestError("Email already in use");
        }

        existingUser = await _userRepository.GetUserByUsername(userCreateDto.Username);
        if (existingUser != null)
        {
            throw new BadRequestError("Username already in use");
        }

        var user = new User
        {
            Username = userCreateDto.Username,
            Email = userCreateDto.Email,
            Password = _passwordHasher.HashPassword(userCreateDto.Password)
        };

        return await _userRepository.CreateUser(user);
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _userRepository.GetUsers();
    }

    public async Task<User> GetUserById(Guid id)
    {
        var user = await _userRepository.GetUserById(id) ?? throw new NotFoundError("User not found");
        return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetUserByEmail(email) ?? throw new NotFoundError("User not found");
        return user;
    }

    public async Task<User> GetUserByUsername(string username)
    {
        var user = await _userRepository.GetUserByUsername(username) ?? throw new NotFoundError("User not found");
        return user;
    }

    public async Task<User> UpdateUser(Guid id, UserUpdateDto userUpdateDto)
    {
        _ = await _userRepository.GetUserById(id) ?? throw new NotFoundError("User not found");
        var updates = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(userUpdateDto.Username))
        {
            updates.Add(nameof(userUpdateDto.Username), userUpdateDto.Username);
        }

        if (!string.IsNullOrEmpty(userUpdateDto.Password))
        {
            var hashedPassword = _passwordHasher.HashPassword(userUpdateDto.Password);
            updates.Add(nameof(userUpdateDto.Password), hashedPassword);
        }

        if (!string.IsNullOrEmpty(userUpdateDto.FirstName))
        {
            updates.Add(nameof(userUpdateDto.FirstName), userUpdateDto.FirstName);
        }

        if (!string.IsNullOrEmpty(userUpdateDto.LastName))
        {
            updates.Add(nameof(userUpdateDto.LastName), userUpdateDto.LastName);
        }

        if (!string.IsNullOrEmpty(userUpdateDto.Avatar))
        {
            updates.Add(nameof(userUpdateDto.Avatar), userUpdateDto.Avatar);
        }

        return await _userRepository.UpdateUser(id, updates) ?? throw new NotFoundError("User not found");
    }

    public async Task<User> DeleteUser(Guid id)
    {
        return await _userRepository.DeleteUser(id) ?? throw new NotFoundError("User not found");
    }

    public async Task<User> UpdateRefreshToken(Guid id, string? refreshToken)
    {
        var user = await _userRepository.GetUserById(id) ?? throw new NotFoundError("User not found");
        user.RefreshToken = refreshToken;
        return await _userRepository.UpdateUser(id, new Dictionary<string, object> { { nameof(user.RefreshToken), refreshToken } });
    }

}
