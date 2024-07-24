#region

using Blake3;

#endregion

namespace DropBear.Codex.Hashing.Hashers;

public class ExtendedBlake3Hasher : Blake3Hasher
{
    public static string IncrementalHash(IEnumerable<byte[]> dataSegments)
    {
        if (dataSegments is null)
        {
            throw new ArgumentNullException(nameof(dataSegments), "Data segments cannot be null.");
        }

        using var hasher = Hasher.New();
        foreach (var segment in dataSegments)
        {
            if (segment is null)
#pragma warning disable CA2208
#pragma warning disable MA0015
            {
                throw new ArgumentNullException(nameof(segment), "Data segment cannot be null.");
            }
#pragma warning restore MA0015
#pragma warning restore CA2208
            hasher.Update(segment);
        }

        return hasher.Finalize().ToString();
    }

    public static string GenerateMac(byte[] data, byte[] key)
    {
        if (key.Length is not 32)
        {
            throw new ArgumentException("Key must be 256 bits (32 bytes).", nameof(key));
        }

        if (data is null)
        {
            throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        }

        using var hasher = Hasher.NewKeyed(key);
        hasher.Update(data);
        return hasher.Finalize().ToString();
    }

    public static byte[] DeriveKey(byte[] context, byte[] inputKeyingMaterial)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context), "Context cannot be null.");
        }

        if (inputKeyingMaterial is null)
        {
            throw new ArgumentNullException(nameof(inputKeyingMaterial), "Input keying material cannot be null.");
        }

        using var hasher = Hasher.NewDeriveKey(context);
        hasher.Update(inputKeyingMaterial);

        // Assuming Finalize returns a Hash object which supports AsSpan or similar
        var hashResult = hasher.Finalize();
        return hashResult.AsSpan().ToArray(); // Convert the ReadOnlySpan<byte> to byte[]
    }

    public static string HashStream(Stream inputStream)
    {
        if (inputStream is null)
        {
            throw new ArgumentNullException(nameof(inputStream), "Input stream cannot be null.");
        }

        using var hasher = Hasher.New();
        var buffer = new byte[4096]; // Buffer size can be adjusted based on needs.
        int bytesRead;
        while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            hasher.Update(buffer.AsSpan(0, bytesRead));
        }

        return hasher.Finalize().ToString();
    }
}
