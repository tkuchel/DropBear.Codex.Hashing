using System.Text;
using Blake3;
using DropBear.Codex.Core;
using DropBear.Codex.Hashing.Interfaces;

namespace DropBear.Codex.Hashing.Hashers;

public class Blake3Hasher : IHasher
{
    // Blake3 does not use salt or iterations, so related methods are no-ops but included for interface compatibility
    public IHasher WithSalt(byte[]? salt) => this;
    public IHasher WithIterations(int iterations) => this;

    public Result<string> Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return Result<string>.Failure("Input cannot be null or empty.");

        try
        {
            var hash = Hasher.Hash(Encoding.UTF8.GetBytes(input)).ToString();
            return Result<string>.Success(hash);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during hashing: {ex.Message}");
        }
    }

    public Result Verify(string input, string expectedHash)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(expectedHash))
            return Result.Failure("Input and expected hash cannot be null or empty.");

        try
        {
            var hash = Hasher.Hash(Encoding.UTF8.GetBytes(input)).ToString();
            return hash == expectedHash ? Result.Success() : Result.Failure("Verification failed.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error during verification: {ex.Message}");
        }
    }

    public Result<string> EncodeToBase64Hash(byte[] data)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
            return Result<string>.Failure("Data cannot be null or empty.");

        try
        {
            var hash = Hasher.Hash(data);
            return Result<string>.Success(Convert.ToBase64String(hash.AsSpan()));
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error during base64 encoding hash: {ex.Message}");
        }
    }

    public Result VerifyBase64Hash(byte[] data, string expectedBase64Hash)
    {
        if (data == Array.Empty<byte>() || data.Length is 0)
            return Result.Failure("Data cannot be null or empty.");

        try
        {
            var hash = Hasher.Hash(data);
            var base64Hash = Convert.ToBase64String(hash.AsSpan());
            return base64Hash == expectedBase64Hash
                ? Result.Success()
                : Result.Failure("Base64 hash verification failed.");
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error during base64 hash verification: {ex.Message}");
        }
    }
#pragma warning disable IDE0060 // Remove unused parameter
    // Blake3 has a fixed output size but implementing to comply with interface.
    public IHasher WithHashSize(int size) => this;
#pragma warning restore IDE0060 // Remove unused parameter
}
