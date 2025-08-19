using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using ShipJobPortal.Domain.Interfaces;

namespace ShipJobPortal.Infrastructure.Helpers;

public class EncryptionHelper : IEncryptionService
{
    private readonly byte[] Key;
    private readonly byte[] IV;

    public EncryptionHelper(IConfiguration config)
    {
        Key = Encoding.UTF8.GetBytes(config["EncryptionSettings:Key"]);
        IV = Encoding.UTF8.GetBytes(config["EncryptionSettings:IV"]);
    }

    public string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.IV = IV;

        var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cryptoStream))
            sw.Write(plainText);

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(Convert.FromBase64String(encryptedText));
            using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cryptoStream);

            return sr.ReadToEnd();
        }
        catch
        {
            return encryptedText; // Return the original text if it's not encrypted
        }
    }
}
