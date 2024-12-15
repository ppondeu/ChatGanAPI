using ChatApi.Entities;

namespace ChatApi.Interfaces;

public interface IUserRepository
{
    Task<User> CreateUser(User user);
    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByUsername(string username);
    Task<IEnumerable<User>> GetUsers();
    Task<User?> UpdateUser(Guid id, Dictionary<string, object> updates);
    Task<User?> DeleteUser(Guid id);
    Task<User?> GetUserByRefreshToken(string refreshToken);
    Task<User?> UpdateRefreshToken(Guid id, string? refreshToken);

}
