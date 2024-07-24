#region

using System.Collections;
using System.Text;
using DropBear.Codex.Core;
using DropBear.Codex.Hashing.Helpers;
using DropBear.Codex.Hashing.Interfaces;
using Konscious.Security.Cryptography;

#endregion

namespace DropBear.Codex.Hashing.Hashers;

public class Argon2Hasher : IHasher
{
    private int _degreeOfParallelism = 8;
    private int _hashSize = 16;
    private int _iterations = 4;
    private int _memorySize = 1024 * 1024; // 1GB
    private byte[]? _salt;

    public IHasher WithSalt(byte[]? salt)
    {
        _salt = salt ?? throw new ArgumentNullException(nameof(salt), "Salt cannot be null.");
        return this;
    }

    public IHasher WithIterations(int iterations)
    {
        if (iterations < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(iterations), "Iterations must be at least 1.");
        }

        _iterations = iterations;
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
            _salt ??= HashingHelper.GenerateRandomSalt(32);
            using var argon2 = CreateArgon2(input, _salt);
            var hashBytes = argon2.GetBytes(_hashSize);
            var combinedBytes = HashingHelper.CombineBytes(_salt, hashBytes);
            return Result<string>.Success(Convert.ToBase64String(combinedBytes));
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error computing hash: {ex.Message}");
        }
    }

    public Result Verify(string input, string expectedHash)
    {
        try
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(expectedHash))
            {
                return Result.Failure("Input and expected hash cannot be null or empty.");
            }

            if (_salt is null)
            {
                return Result.Failure("Salt is required for verification.");
            }

            var expectedBytes = Convert.FromBase64String(expectedHash);
            var (salt, expectedHashBytes) = HashingHelper.ExtractBytes(expectedBytes, _salt.Length);
            using var argon2 = CreateArgon2(input, salt);
            var hashBytes = argon2.GetBytes(_hashSize);

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
        {
            return Result<string>.Failure("Data cannot be null or empty.");
        }

        return Result<string>.Success(Convert.ToBase64String(data));
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
        {
            return Result.Failure("Data cannot be null or empty.");
        }

        var base64Hash = Convert.ToBase64String(data);
        return base64Hash == expectedBase64Hash ? Result.Success() : Result.Failure("Base64 hash verification failed.");
    }

    public IHasher WithHashSize(int size)
    {
        if (size < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Hash size must be at least 1 byte.");
        }

        _hashSize = size;
        return this;
    }

    public IHasher WithDegreeOfParallelism(int degree)
    {
        if (degree < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(degree), "Degree of parallelism must be at least 1.");
        }

        _degreeOfParallelism = degree;
        return this;
    }

    public IHasher WithMemorySize(int size)
    {
        if (size < 1024)
        {
            throw new ArgumentOutOfRangeException(nameof(size), "Memory size must be at least 1024 bytes (1KB).");
        }

        _memorySize = size;
        return this;
    }

    private Argon2id CreateArgon2(string input, byte[]? salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(input))
        {
            Salt = salt,
            DegreeOfParallelism = _degreeOfParallelism,
            Iterations = _iterations,
            MemorySize = _memorySize
        };
        return argon2;
    }
}
