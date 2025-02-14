using Isopoh.Cryptography.Argon2;

namespace Dobrasync.Common.Util;

public static class PasswordHashUtil
{
    public static string HashPassword(string password)
    {
        return Argon2.Hash(password);
    }
    
    public static bool Verify(string password, string hash)
    {
        return Argon2.Verify(hash, password);
    }
}