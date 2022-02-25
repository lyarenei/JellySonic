using System;
using System.IO;

namespace JellySonic.Utils;

/// <summary>
/// A collection of util functions for JellySonic.
/// </summary>
public static class Utils
{
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
}
