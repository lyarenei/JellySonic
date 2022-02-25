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
        Dictionary<Type, int> typeDict = new Dictionary<Type, int>
        {
            { typeof(AlbumResponseData), 0 },
            { typeof(ArtistResponseData), 1 },
            { typeof(ArtistsResponseData), 2 },
            { typeof(DirectoryResponseData), 3 },
            { typeof(ErrorResponseData), 4 },
            { typeof(LicenseResponseData), 5 },
            { typeof(MusicFoldersResponseData), 6 },
            { typeof(SongResponseData), 7 }
        };

        writer.WriteStartObject();

        writer.WriteString("status", value.Status);
        writer.WriteString("version", value.Version);

        if (value.ResponseData != null)
        {
            switch (typeDict[value.ResponseData.GetType()])
            {
                case 0:
                    writer.WritePropertyName("album");
                    break;
                case 1:
                    writer.WritePropertyName("artist");
                    break;
                case 2:
                    writer.WritePropertyName("artists");
                    break;
                case 3:
                    writer.WritePropertyName("directory");
                    break;
                case 4:
                    writer.WritePropertyName("error");
                    break;
                case 5:
                    writer.WritePropertyName("license");
                    break;
                case 6:
                    writer.WritePropertyName("musicFolder");
                    break;
                case 7:
                    writer.WritePropertyName("song");
                    break;
            }

            JsonSerializer.Serialize(writer, value.ResponseData, value.ResponseData.GetType(), options);
        }

        writer.WriteEndObject();
    }
}
