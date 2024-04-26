using DropBear.Codex.Hashing.Hashers;
using DropBear.Codex.Hashing.Interfaces;

namespace DropBear.Codex.Hashing;

/// <summary>
///     Provides a flexible way to construct and retrieve hasher instances by key.
/// </summary>
public class HashBuilder : IHashBuilder
{
    private readonly Dictionary<string, Func<IHasher>> _serviceConstructors;

    /// <summary>
    ///     Initializes a new instance of the HashBuilder class and configures default hasher services.
    /// </summary>
    public HashBuilder() =>
        _serviceConstructors = new Dictionary<string, Func<IHasher>>(StringComparer.OrdinalIgnoreCase)
        {
            { "argon2", () => new Argon2Hasher() },
            { "blake2", () => new Blake2Hasher() },
            { "blake3", () => new Blake3Hasher() },
            { "fnv1a", () => new Fnv1AHasher() },
            { "murmur3", () => new Murmur3Hasher() },
            { "siphash", () => new SipHasher(new byte[16]) }, // Assumes the key is predefined and static
            { "xxhash", () => new XxHasher() },
            { "extended_blake3", () => new ExtendedBlake3Hasher() } // Extended Blake3 Service
        };

    /// <summary>
    ///     Retrieves a hasher instance based on the specified key.
    /// </summary>
    /// <param name="key">The key identifying the hasher service.</param>
    /// <returns>The corresponding hasher instance.</returns>
    /// <exception cref="ArgumentException">Thrown if no hasher is registered with the provided key.</exception>
    public IHasher GetHasher(string key)
    {
        if (!_serviceConstructors.TryGetValue(key, out var constructor))
            throw new ArgumentException($"No hashing service registered for key: {key}", nameof(key));
        return constructor();
    }
}
