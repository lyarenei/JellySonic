using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace JellySonic.Models;

/// <summary>
/// Subsonic request parameters.
/// </summary>
public class SubsonicParams
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicParams"/> class.
    /// </summary>
    /// <param name="data">Request data.</param>
    public SubsonicParams(IEnumerable<KeyValuePair<string, StringValues>> data)
    {
        var missing = new KeyValuePair<string, StringValues>(string.Empty, default);
        var dataArray = data as KeyValuePair<string, StringValues>[] ?? data.ToArray();

        Username = dataArray.FirstOrDefault(kvp => kvp.Key == "u", missing).Value;
        Password = dataArray.FirstOrDefault(kvp => kvp.Key == "p", missing).Value;
        Token = dataArray.FirstOrDefault(kvp => kvp.Key == "t", missing).Value;
        Salt = dataArray.FirstOrDefault(kvp => kvp.Key == "s", missing).Value;
        Version = dataArray.FirstOrDefault(kvp => kvp.Key == "v", missing).Value;
        Client = dataArray.FirstOrDefault(kvp => kvp.Key == "c", missing).Value;
        Format = dataArray.FirstOrDefault(kvp => kvp.Key == "f", missing).Value;

        Id = dataArray.FirstOrDefault(kvp => kvp.Key == "id", missing).Value;
        MusicFolderId = dataArray.FirstOrDefault(kvp => kvp.Key == "musicFolderId", missing).Value;
        IfModifiedSince = dataArray.FirstOrDefault(kvp => kvp.Key == "ifModifiedSince", missing).Value;
        Count = dataArray.FirstOrDefault(kvp => kvp.Key == "count", missing).Value;
        IncludeNotPresent = dataArray.FirstOrDefault(kvp => kvp.Key == "includeNotPresent", missing).Value;
        Artist = dataArray.FirstOrDefault(kvp => kvp.Key == "artist", missing).Value;
        Type = dataArray.FirstOrDefault(kvp => kvp.Key == "type", missing).Value;
        Size = dataArray.FirstOrDefault(kvp => kvp.Key == "size", missing).Value;
        Offset = dataArray.FirstOrDefault(kvp => kvp.Key == "offset", missing).Value;
        FromYear = dataArray.FirstOrDefault(kvp => kvp.Key == "fromYear", missing).Value;
        ToYear = dataArray.FirstOrDefault(kvp => kvp.Key == "toYear", missing).Value;
        Genre = dataArray.FirstOrDefault(kvp => kvp.Key == "genre", missing).Value;
        Query = dataArray.FirstOrDefault(kvp => kvp.Key == "query", missing).Value;
        ArtistCount = dataArray.FirstOrDefault(kvp => kvp.Key == "artistCount", missing).Value;
        ArtistOffset = dataArray.FirstOrDefault(kvp => kvp.Key == "artistOffset", missing).Value;
        AlbumCount = dataArray.FirstOrDefault(kvp => kvp.Key == "albumCount", missing).Value;
        AlbumOffset = dataArray.FirstOrDefault(kvp => kvp.Key == "albumOffset", missing).Value;
        SongCount = dataArray.FirstOrDefault(kvp => kvp.Key == "songCount", missing).Value;
        SongOffset = dataArray.FirstOrDefault(kvp => kvp.Key == "songOffset", missing).Value;
    }

    /// <summary>
    /// Determines if there's data necessary to perform token authentication.
    /// </summary>
    /// <returns>Token authentication is possible.</returns>
    public bool TokenAuthPossible()
    {
        return !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(Salt);
    }

    /// <summary>
    /// Check if required request parameters are missing.
    /// </summary>
    /// <returns>One or more required parameters are missing.</returns>
    public bool RequiredParamsMissing()
    {
        // Note: some clients do not send format parameter, so it's left out.
        var authPossible = TokenAuthPossible() || !string.IsNullOrEmpty(Password);
        return string.IsNullOrEmpty(Username) ||
               string.IsNullOrEmpty(Version) ||
               string.IsNullOrEmpty(Client) ||
               !authPossible;
    }

    /// <summary>
    /// Check if required request parameters are missing.
    /// </summary>
    /// <param name="additionalParams">Additional parameter names which must be set.</param>
    /// <returns>A name of required parameter which has no value. Empty if all parameters have value.</returns>
    public string RequiredParamsMissing(params string[] additionalParams)
    {
        var lowercaseAdditionalParams = additionalParams.Select(param => param.ToLower(CultureInfo.InvariantCulture)).ToList();
        foreach (var property in this.GetType().GetProperties())
        {
            var propValue = (string?)property.GetValue(this);
            var propName = property.Name.ToLower(CultureInfo.InvariantCulture);
            if (
                string.IsNullOrEmpty(propValue) &&
                lowercaseAdditionalParams.Contains(propName)
            )
            {
                return propName;
            }
        }

        return RequiredParamsMissing() ? "one of common parameters (username, client, etc...)" : string.Empty;
    }

    /// <summary>
    /// Get subsonic password. Password is automatically decoded if necessary.
    /// </summary>
    /// <returns>Subsonic user password.</returns>
    public string RetrievePassword()
    {
        if (Password.Contains("enc:", StringComparison.InvariantCulture))
        {
            return Utils.Utils.HexToAscii(Password[4..]);
        }

        return Password;
    }

    /// <summary>
    /// Verify provided token for subsonic authentication.
    /// </summary>
    /// <returns>Token is valid.</returns>
    public bool VerifyToken()
    {
        var computedToken = Utils.Utils.Md5Hash(Password + Salt).ToLower(CultureInfo.InvariantCulture);
        return Token == computedToken;
    }

#pragma warning disable CS1591
#pragma warning disable SA1201

    public string SongOffset { get; }

    public string SongCount { get; }

    public string AlbumOffset { get; }

    public string AlbumCount { get; }

    public string ArtistOffset { get; }

    public string ArtistCount { get; }

    public string Query { get; }

    public string Genre { get; }

    public string ToYear { get; }

    public string FromYear { get; }

    public string Offset { get; }

    public string Size { get; }

    public string Type { get; }

    public string IncludeNotPresent { get; }

    public string Count { get; }

    public string IfModifiedSince { get; }

    public string MusicFolderId { get; }

    public string Id { get; }

    public string Artist { get; }

    public string Version { get; }

    public string Salt { get; }

    public string Token { get; }

    public string Username { get; }

    public string Password { get; }

    public string Client { get; }

    public string Format { get; }

#pragma warning restore SA1201
#pragma warning restore CS1591
}
