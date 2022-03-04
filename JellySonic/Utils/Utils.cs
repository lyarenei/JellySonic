using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JellySonic.Utils;

/// <summary>
/// A collection of util functions for JellySonic.
/// </summary>
public static class Utils
{
    /// <summary>
    /// ISO date format.
    /// </summary>
    /// <returns>ISO date format string.</returns>
    public const string IsoDateFormat = "yyyy-MM-ddTHH:mm:ssK";

    /// <summary>
    /// Convert ticks to seconds.
    /// </summary>
    /// <param name="ticks">Ticks value.</param>
    /// <returns>Ticks value converted to seconds. 0 if cannot convert.</returns>
    public static long TicksToSeconds(long? ticks)
    {
        return (ticks / TimeSpan.TicksPerSecond) ?? 0;
    }

    /// <summary>
    /// Get MIME type from the filename extension.
    /// </summary>
    /// <param name="filename">Path to the file.</param>
    /// <returns>MIME type string.</returns>
    public static string GetMimeType(string filename)
    {
        return Path.GetExtension(filename) switch
        {
            ".flac" => "audio/flac",
            ".mp3" => "audio/mpeg",
            ".ogg" => "audio/ogg",
            ".opus" => "audio/opus",
            ".wav" => "audio/wave",
            _ => "audio/basic"
        };
    }

    /// <summary>
    /// Decode hex-encoded string to ascii.
    /// </summary>
    /// <param name="hexString">String to decode.</param>
    /// <returns>Decoded string.</returns>
    public static string HexToAscii(string hexString)
    {
        var bytes = Convert.FromHexString(hexString);
        return bytes.Aggregate(string.Empty, (current, b) => current + (char)b);
    }

    /// <summary>
    /// Computes a md5 hash of input value.
    /// </summary>
    /// <param name="input">Value to hash.</param>
    /// <returns>Hash of input. Null if error.</returns>
    [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms", Justification = "Subsonic authentication.")]
    public static string Md5Hash(string input)
    {
        // From https://stackoverflow.com/a/24031467
        using (var md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes)
                .Replace("-", string.Empty, StringComparison.InvariantCulture);
        }
    }
}
