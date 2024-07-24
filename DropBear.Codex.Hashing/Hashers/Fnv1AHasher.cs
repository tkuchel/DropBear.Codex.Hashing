#region

using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Hashing.Interfaces;
using HashDepot;

#endregion

namespace DropBear.Codex.Hashing.Hashers;

public class Fnv1AHasher : IHasher
{
    // FNV-1a does not utilize salt or iterations, so the related methods are no-ops but are implemented for interface compliance.
    public IHasher WithSalt(byte[]? salt)
    {
        return this;
    }

    public IHasher WithIterations(int iterations)
    {
        return this;
    }

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return Result<string>.Failure("Input cannot be null or empty.");
        }

        try
        {
            var buffer = Encoding.UTF8.GetBytes(input);
            var hash = Fnv1a.Hash32(buffer); // Using 32-bit hash for demonstration purposes
            return Result<string>.Success(hash.ToString("x8"));
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during hashing: {ex.Message}");
        }
    }

    public Result Verify(string input, string expectedHash)
    {
        var hashResult = Hash(input);
        if (!hashResult.IsSuccess)
        {
            return Result.Failure("Failed to compute hash.");
        }

        var isValid = string.Equals(hashResult.Value, expectedHash, StringComparison.OrdinalIgnoreCase);
        return isValid ? Result.Success() : Result.Failure("Verification failed.");
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
        {
            return Result<string>.Failure("Data cannot be null or empty.");
        }

        try
        {
            var hash = Fnv1a.Hash64(data); // Using 64-bit hash for broader applicability
            var hashBytes = BitConverter.GetBytes(hash);
            var base64Hash = Convert.ToBase64String(hashBytes);
            return Result<string>.Success(base64Hash);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during base64 encoding hash: {ex.Message}");
        }
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        var encodeResult = EncodeToBase64Hash(data);
        if (!encodeResult.IsSuccess)
        {
            return Result.Failure("Failed to compute hash.");
        }

        var isValid = string.Equals(encodeResult.Value, expectedBase64Hash, StringComparison.Ordinal);
        return isValid ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }


#pragma warning disable IDE0060 // Remove unused parameter
    // FNV-1a output size is determined by the algorithm (32-bit or 64-bit), this method is effectively a noop.
    public IHasher WithHashSize(int size)
    {
        return this;
    }
#pragma warning restore IDE0060 // Remove unused parameter
}
