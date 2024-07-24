# DropBear.Codex.Hashing Library

## Overview

This library provides a flexible and extensible hashing framework, designed for easy integration and usage within .NET
applications. It supports a variety of hashing algorithms and offers a fluent API for hasher configurations.

## Supported Hash Algorithms

- **Argon2Hasher**: Suitable for password hashing with configurable parameters like salt, iterations, and memory usage.
- **Blake2Hasher**: Provides a cryptographic hash function with high security and speed.
- **Blake3Hasher**: An evolution of Blake2, offers faster hashing speeds and is suitable for both cryptographic and
  general-purpose use.
- **Fnv1AHasher**: Implements the FNV-1a hash function, which is effective for hashing short strings.
- **MurmurHash3Service**: Offers a non-cryptographic hash function known for its speed and distribution properties.
- **SipHasher**: Designed to be a fast hash function that is secure against hash-flooding DoS attacks.
- **XxHasher**: Extremely fast hashing, useful for checksums and hash tables.
- **ExtendedBlake3Hasher**: Extends Blake3 capabilities, supporting incremental hashing, keyed hashing, and deriving
  cryptographic keys.

## Features

- **Flexible Hashing**: Easy to switch between different hashing algorithms using a simple key-based retrieval system.
- **Fluent Configuration**: Hashers can be fluently configured with salts, iterations, and other specific parameters.
- **Extensibility**: New hashing algorithms can be easily integrated into the library.

## Usage

### Basic Usage

```csharp
using DropBear.Codex.Utilities.Hashing;

var hashBuilder = new HashBuilder();
var hasher = hashBuilder.GetHasher("blake3");
var hash = hasher.Hash("example input");

if (hash.IsSuccess)
{
    Console.WriteLine($"Hashed Output: {hash.Value}");
}
else
{
    Console.WriteLine("Hashing failed.");
}
```

### Configuring a Hasher

```csharp
var hasher = hashBuilder.GetHasher("argon2").WithSalt(someSalt).WithIterations(10);
var hash = hasher.Hash("secure input");
```

## Installation

To integrate `DropBear.Codex.Hashing` into your project, add the library as a dependency via your project's package
management system.

## Contributions

Contributions are welcome. Please submit pull requests or issues to our GitHub repository to suggest enhancements or
report bugs.

## License

`DropBear.Codex.Hashing` is licensed under the MIT license. See the LICENSE file in the source repository for more
details.
