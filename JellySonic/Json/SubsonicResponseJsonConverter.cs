using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JellySonic.Models;

namespace JellySonic.Json;

/// <summary>
/// Custom converter for <see cref="SubsonicResponse"/>.
/// </summary>
public class SubsonicResponseJsonConverter : JsonConverter<SubsonicResponse>
{
    /// <inheritdoc />
    public override SubsonicResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, SubsonicResponse value, JsonSerializerOptions options)
    {
        Dictionary<Type, string> typeDict = new Dictionary<Type, string>
        {
            { typeof(AlbumWithSongsId3), "album" },
            { typeof(ArtistWithAlbumsId3), "artist" },
            { typeof(ArtistsId3), "artists" },
            { typeof(License), "license" },
            { typeof(Child), "song" },
            { typeof(MusicFolders), "musicFolders" },
            { typeof(Directory), "directory" },
            { typeof(SubsonicError), "error" },
        };

        writer.WriteStartObject();

        writer.WritePropertyName("subsonic-response");
        writer.WriteStartObject();
        writer.WriteString("status", value.Status);
        writer.WriteString("version", value.Version);

        if (value.ResponseData != null)
        {
            writer.WritePropertyName(typeDict[value.ResponseData.GetType()]);
            JsonSerializer.Serialize(writer, value.ResponseData, value.ResponseData.GetType(), options);
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
    }
}
