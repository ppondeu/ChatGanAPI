using System;
using ChatApi.Data;
using ChatApi.Entities;
using ChatApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<User> CreateUser(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> DeleteUser(Guid id)
    {
        var existingUser = _context.Users.Find(id);
        if (existingUser == null)
        {
            return null;
        }
        _context.Users.Remove(existingUser);
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByRefreshToken(string refreshToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> UpdateRefreshToken(Guid id, string? refreshToken)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return null;
        }
        existingUser.RefreshToken = refreshToken;
        await _context.SaveChangesAsync();
        return existingUser;
    }

    public async Task<User?> UpdateUser(Guid id, Dictionary<string, object> updates)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
        {
            return null;
        }
        foreach (var (key, value) in updates)
        {
            var property = existingUser.GetType().GetProperty(key);
            if (property != null && property.CanWrite)
            {
                property.SetValue(existingUser, value);
            }
        }
        await _context.SaveChangesAsync();
        return existingUser;
    }
}
