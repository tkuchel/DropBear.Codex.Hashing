using System.Collections;
using System.Text;
using Blake2Fast;
using DropBear.Codex.Core;
using DropBear.Codex.Hashing.Helpers;
using DropBear.Codex.Hashing.Interfaces;

namespace DropBear.Codex.Hashing.Hashers;

public class Blake2Hasher : IHasher
{
    private int _hashSize = 32; // Default hash size for Blake2b
    private byte[]? _salt;

    public IHasher WithSalt(byte[]? salt)
    {
        if (salt is null || salt.Length is 0)
            throw new ArgumentException("Salt cannot be null or empty.", nameof(salt));
        _salt = salt;
        return this;
    }

    public IHasher WithIterations(int iterations) => this;

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        _salt = _salt ?? HashingHelper.GenerateRandomSalt(32);
        var hashBytes = HashWithBlake2(input, _salt);
        var combinedBytes = HashingHelper.CombineBytes(_salt, hashBytes);
        return Result<string>.Success(Convert.ToBase64String(combinedBytes));
    }

    public Result Verify(string input, string expectedHash)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(expectedHash))
            return Result.Failure("Input and expected hash cannot be null or empty.");

        if (_salt is null)
            return Result.Failure("Salt is required for verification.");

        try
        {
            var expectedBytes = Convert.FromBase64String(expectedHash);
            var (salt, expectedHashBytes) = HashingHelper.ExtractBytes(expectedBytes, _salt.Length);
            var hashBytes = HashWithBlake2(input, salt);

            var isValid = StructuralComparisons.StructuralEqualityComparer.Equals(hashBytes, expectedHashBytes);
            return isValid ? Result.Success() : Result.Failure("Verification failed.");
        }
        catch (FormatException)
        {
            return Result.Failure("Expected hash format is invalid.");
        }
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        var hash = Blake2b.ComputeHash(_hashSize, data);
        return Result<string>.Success(Convert.ToBase64String(hash));
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
            return Result.Failure("Data cannot be null or empty.");

        var hash = Blake2b.ComputeHash(_hashSize, data);
        var base64Hash = Convert.ToBase64String(hash);

        return base64Hash == expectedBase64Hash ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }

    public IHasher WithHashSize(int size)
    {
        if (size < 1)
            throw new ArgumentOutOfRangeException(nameof(size), "Hash size must be at least 1 byte.");
        _hashSize = size;
        return this;
    }

    private byte[] HashWithBlake2(string input, byte[]? salt)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var saltedInput = HashingHelper.CombineBytes(salt, inputBytes);
        return Blake2b.ComputeHash(_hashSize, saltedInput);
    }
}
