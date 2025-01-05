using System;
using ChatApi.DTOs;
using ChatApi.Entities;

namespace ChatApi.Interfaces;

public interface IUserService
{
    Task<User> CreateUser(UserCreateDto userCreateDto);
    Task<IEnumerable<User>> GetUsers();
    Task<IEnumerable<User>> GetFriends(Guid userId);
    Task<User> GetUserById(Guid id);
    Task<User> GetUserByEmail(string email);
    Task<User> GetUserByUsername(string username);
    Task<User> UpdateUser(Guid id, UserUpdateDto userUpdateDto);
    Task<User> DeleteUser(Guid id);
    Task<User> UpdateRefreshToken(Guid id, string? refreshToken);
}
