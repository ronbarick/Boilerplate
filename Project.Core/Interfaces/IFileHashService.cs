namespace Project.Core.Interfaces;

/// <summary>
/// Service for computing and verifying file hashes
/// </summary>
public interface IFileHashService
{
    /// <summary>
    /// Compute SHA256 hash of a stream
    /// </summary>
    Task<string> ComputeHashAsync(Stream stream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify that a stream matches the expected hash
    /// </summary>
    Task<bool> VerifyHashAsync(Stream stream, string expectedHash, CancellationToken cancellationToken = default);
}
