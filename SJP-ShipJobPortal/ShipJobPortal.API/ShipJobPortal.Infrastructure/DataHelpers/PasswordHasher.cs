using System.Security.Cryptography;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Infrastructure.DataHelpers;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        using var rng = new RNGCryptoServiceProvider();
        byte[] salt = new byte[16];
        rng.GetBytes(salt);

        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(20);

        byte[] hashBytes = new byte[36];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 20);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string enteredPassword, string storedHash)
    {
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(20);

        for (int i = 0; i < 20; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }

        return true;
    }
}
