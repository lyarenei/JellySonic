using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Serialization;
using Jellyfin.Data.Entities;
using JellySonic.Types;

namespace JellySonic.Models;

/// <summary>
/// A Subsonic user response.
/// </summary>
public class SubsonicUser : IResponseData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicUser"/> class.
    /// </summary>
    public SubsonicUser()
    {
        Folders = new Collection<int>();
        Username = string.Empty;
        ScrobblingEnabled = "false";
        AdminRole = "false";
        SettingsRole = "false";
        DownloadRole = "false";
        UploadRole = "false";
        PlaylistRole = "false";
        CoverArtRole = "false";
        CommentRole = "false";
        PodcastRole = "false";
        StreamRole = "false";
        JukeboxRole = "false";
        ShareRole = "false";
        VideoConversionRole = "false";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubsonicUser"/> class.
    /// </summary>
    /// <param name="user">Jellyfin user.</param>
    public SubsonicUser(User user)
    {
        Folders = new Collection<int>();
        Username = user.Username;
        ScrobblingEnabled = "false";
        AdminRole = "false";
        SettingsRole = "false";
        DownloadRole = "true";
        UploadRole = "false";
        PlaylistRole = "false";
        CoverArtRole = "true";
        CommentRole = "false";
        PodcastRole = "false";
        StreamRole = "true";
        JukeboxRole = "false";
        ShareRole = "false";
        VideoConversionRole = "false";
        AvatarLastChanged = user.ProfileImage?.LastModified.ToString(Utils.Utils.IsoDateFormat, DateTimeFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Gets folders user has an access to.
    /// </summary>
    [XmlText]
    [XmlElement("folder")]
    public Collection<int> Folders { get; }

    /// <summary>
    /// Gets name of the user.
    /// </summary>
    [XmlAttribute("username")]
    public string Username { get; }

    /// <summary>
    /// Gets user e-mail.
    /// </summary>
    [XmlAttribute("email")]
    public string? Email { get; }

    /// <summary>
    /// Gets if user has enabled scrobbling.
    /// </summary>
    [XmlAttribute("scrobblingEnabled")]
    public string ScrobblingEnabled { get; }

    /// <summary>
    /// Gets maximum bitrate allowed. In kbps.
    /// </summary>
    [XmlAttribute("maxBitRate")]
    public string? MaxBitRate { get; }

    /// <summary>
    /// Gets if user has admin role.
    /// </summary>
    [XmlAttribute("adminRole")]
    public string AdminRole { get; }

    /// <summary>
    /// Gets if user has settings role.
    /// </summary>
    [XmlAttribute("settingsRole")]
    public string SettingsRole { get; }

    /// <summary>
    /// Gets if user has download role.
    /// </summary>
    [XmlAttribute("downloadRole")]
    public string DownloadRole { get; }

    /// <summary>
    /// Gets if user has upload role.
    /// </summary>
    [XmlAttribute("uploadRole")]
    public string UploadRole { get; }

    /// <summary>
    /// Gets if user has playlist role.
    /// </summary>
    [XmlAttribute("playlistRole")]
    public string PlaylistRole { get; }

    /// <summary>
    /// Gets if user has cover art role.
    /// </summary>
    [XmlAttribute("coverArtRole")]
    public string CoverArtRole { get; }

    /// <summary>
    /// Gets if user has comment role.
    /// </summary>
    [XmlAttribute("commentRole")]
    public string CommentRole { get; }

    /// <summary>
    /// Gets if user has podcast role.
    /// </summary>
    [XmlAttribute("podcastRole")]
    public string PodcastRole { get; }

    /// <summary>
    /// Gets uf user has stream role.
    /// </summary>
    [XmlAttribute("streamRole")]
    public string StreamRole { get; }

    /// <summary>
    /// Gets if user has jukebox role.
    /// </summary>
    [XmlAttribute("jukeboxRole")]
    public string JukeboxRole { get; }

    /// <summary>
    /// Gets if user has share role.
    /// </summary>
    [XmlAttribute("shareRole")]
    public string ShareRole { get; }

    /// <summary>
    /// Gets if user has video conversion role.
    /// </summary>
    [XmlAttribute("videoConversionRole")]
    public string VideoConversionRole { get; }

    /// <summary>
    /// Gets last date when user changed their avatar.
    /// </summary>
    [XmlAttribute("avatarLastChanged")]
    public string? AvatarLastChanged { get; }
}
