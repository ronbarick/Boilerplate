using System.Security.Cryptography;
using System.Text;
using Project.Core.Interfaces;

namespace Project.Infrastructure.Services.FileStorage;

/// <summary>
/// Service for computing SHA256 hashes of files
/// </summary>
public class FileHashService : IFileHashService
{
    public async Task<string> ComputeHashAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        // Ensure stream is at the beginning
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(stream, cancellationToken);

        // Reset stream position for subsequent operations
        if (stream.CanSeek)
        {
            stream.Position = 0;
        }

        // Convert to hex string
        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

    public async Task<bool> VerifyHashAsync(Stream stream, string expectedHash, CancellationToken cancellationToken = default)
    {
        var actualHash = await ComputeHashAsync(stream, cancellationToken);
        return string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
