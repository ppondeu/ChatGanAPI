using ChatApi.Interfaces;

namespace ChatApi.Helpers;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);
        return hashedPassword;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        bool passwordMatch = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        return passwordMatch;
    }
}
