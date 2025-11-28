using System;
using System.Security.Cryptography;
using System.Text;

namespace Project.Infrastructure.Security;

/// <summary>
/// Helper for TOTP (Time-based One-Time Password) generation and validation.
/// Used for Authenticator App 2FA (Google Authenticator, Authy, etc.)
/// </summary>
public static class TotpHelper
{
    private const int TimeStepSeconds = 30;
    private const int CodeDigits = 6;

    /// <summary>
    /// Generates a random Base32-encoded secret key for TOTP.
    /// </summary>
    public static string GenerateSecret()
    {
        var bytes = new byte[20]; // 160 bits
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Base32Encode(bytes);
    }

    /// <summary>
    /// Generates a QR code URI for Google Authenticator.
    /// </summary>
    public static string GenerateQrCodeUri(string secret, string issuer, string accountName)
    {
        return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(accountName)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}&digits={CodeDigits}&period={TimeStepSeconds}";
    }

    /// <summary>
    /// Validates a TOTP code against a secret.
    /// Allows for time drift (checks current, previous, and next time windows).
    /// </summary>
    public static bool ValidateCode(string secret, string code)
    {
        if (string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(code))
            return false;

        if (code.Length != CodeDigits || !long.TryParse(code, out _))
            return false;

        var currentTimeStep = GetCurrentTimeStep();

        // Check current, previous, and next time windows (allows for 30 second drift)
        for (int i = -1; i <= 1; i++)
        {
            var expectedCode = GenerateCode(secret, currentTimeStep + i);
            if (expectedCode == code)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Generates a TOTP code for the current time.
    /// </summary>
    private static string GenerateCode(string secret, long timeStep)
    {
        var secretBytes = Base32Decode(secret);
        var timeBytes = BitConverter.GetBytes(timeStep);

        if (BitConverter.IsLittleEndian)
            Array.Reverse(timeBytes);

        using var hmac = new HMACSHA1(secretBytes);
        var hash = hmac.ComputeHash(timeBytes);

        var offset = hash[hash.Length - 1] & 0x0F;
        var binary = ((hash[offset] & 0x7F) << 24)
                   | ((hash[offset + 1] & 0xFF) << 16)
                   | ((hash[offset + 2] & 0xFF) << 8)
                   | (hash[offset + 3] & 0xFF);

        var otp = binary % (int)Math.Pow(10, CodeDigits);
        return otp.ToString().PadLeft(CodeDigits, '0');
    }

    private static long GetCurrentTimeStep()
    {
        var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return unixTimestamp / TimeStepSeconds;
    }

    private static string Base32Encode(byte[] data)
    {
        const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        var result = new StringBuilder();
        int buffer = data[0];
        int bitsLeft = 8;
        int index = 1;

        while (bitsLeft > 0 || index < data.Length)
        {
            if (bitsLeft < 5)
            {
                if (index < data.Length)
                {
                    buffer <<= 8;
                    buffer |= data[index++];
                    bitsLeft += 8;
                }
                else
                {
                    int pad = 5 - bitsLeft;
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }

            int value = (buffer >> (bitsLeft - 5)) & 0x1F;
            bitsLeft -= 5;
            result.Append(base32Chars[value]);
        }

        return result.ToString();
    }

    private static byte[] Base32Decode(string base32)
    {
        const string base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        base32 = base32.TrimEnd('=').ToUpper();

        var bytes = new List<byte>();
        int buffer = 0;
        int bitsLeft = 0;

        foreach (char c in base32)
        {
            int value = base32Chars.IndexOf(c);
            if (value < 0)
                throw new ArgumentException("Invalid Base32 character");

            buffer = (buffer << 5) | value;
            bitsLeft += 5;

            if (bitsLeft >= 8)
            {
                bytes.Add((byte)(buffer >> (bitsLeft - 8)));
                bitsLeft -= 8;
            }
        }

        return bytes.ToArray();
    }
}
