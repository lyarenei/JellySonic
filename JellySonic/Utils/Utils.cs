using System;

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
}
