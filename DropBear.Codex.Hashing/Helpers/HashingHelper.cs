using System.Security.Cryptography;

namespace DropBear.Codex.Hashing.Helpers;

public static class HashingHelper
{
    /// <summary>
    ///     Generates a random salt of the optimal size for Blake2b.
    /// </summary>
    /// <returns>The generated salt.</returns>
    public static byte[] GenerateRandomSalt(int saltSize)
    {
        var buffer = new byte[saltSize];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);
        return buffer;
    }

    /// <summary>
    ///     Combines two byte arrays into one.
    /// </summary>
    /// <param name="salt">The first byte array.</param>
    /// <param name="hash">The second byte array.</param>
    /// <returns>The combined byte array.</returns>
    public static byte[] CombineBytes(byte[]? salt, byte[] hash)
    {
        ArgumentNullException.ThrowIfNull(salt);

        var combinedBytes = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, combinedBytes, salt.Length, hash.Length);
        return combinedBytes;
    }

    /// <summary>
    ///     Extracts salt and hash bytes from a combined byte array.
    /// </summary>
    /// <param name="combinedBytes">The combined byte array containing salt and hash bytes.</param>
    /// <param name="saltSize">The size of the salt bytes.</param>
    /// <returns>A tuple containing salt and hash bytes.</returns>
    public static (byte[]? salt, byte[] hash) ExtractBytes(byte[] combinedBytes, int saltSize)
    {
        var salt = new byte[saltSize];
        var hash = new byte[combinedBytes.Length - saltSize];
        Buffer.BlockCopy(combinedBytes, 0, salt, 0, saltSize);
        Buffer.BlockCopy(combinedBytes, saltSize, hash, 0, hash.Length);
        return (salt, hash);
    }

    // Convert byte array to string
    public static string ConvertByteArrayToBase64String(byte[] byteArray) => Convert.ToBase64String(byteArray);

    // Convert string to byte array
    public static byte[] ConvertBase64StringToByteArray(string str) => Convert.FromBase64String(str);
}
