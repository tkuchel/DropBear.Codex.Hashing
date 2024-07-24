#region

using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Hashing.Interfaces;
using HashDepot;

#endregion

namespace DropBear.Codex.Hashing.Hashers;

public class XxHasher : IHasher
{
    private ulong _seed; // Default seed

    public XxHasher(ulong seed = 0)
    {
        _seed = seed;
    }

    // XXHash does not use salt or iterations, so these methods are no-ops but are included for interface compliance
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
            var hash = XXHash.Hash64(buffer, _seed);
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

        return hashResult.Value == expectedHash ? Result.Success() : Result.Failure("Verification failed.");
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
        {
            return Result<string>.Failure("Data cannot be null or empty.");
        }

        try
        {
            var hash = XXHash.Hash64(data, _seed);
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

        return encodeResult.Value == expectedBase64Hash
            ? Result.Success()
            : Result.Failure("Base64 hash verification failed.");
    }

#pragma warning disable IDE0060 // Remove unused parameter
    // XXHash output size is fixed by the algorithm (32-bit or 64-bit), so this method is effectively a noop.
    public IHasher WithHashSize(int size)
    {
        return this;
    }
#pragma warning restore IDE0060 // Remove unused parameter
    public IHasher WithSeed(ulong seed)
    {
        _seed = seed;
        return this;
    }
}
