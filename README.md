# JellySonic
An **experimental** subsonic plugin for Jellyfin.

This plugin enables your Jellyfin server to serve Subsonic clients.

## What is implemented

The state of implementation can be found in the tables below.
Click on each endpoint type reveal the tables.
If you have found any issues, please check the implementation state before opening an issue.

The current scope is to implement such endpoints to allow clients to provide basic/minimal, but reasonable user experience.

The aim of this plugin is to serve the subsonic clients for music playback.
Any management of the server or user(s) is the responsibility of the Jellyfin server.

<details>
  <summary>System</summary>

| endpoint   | implemented | notes |
|------------|-------------|-------|
| ping       | yes         |       |
| getLicense | yes         |       |

</details>

<details>
  <summary>Browsing</summary>

| endpoint          | implemented | notes                         |
|-------------------|-------------|-------------------------------|
| getMusicFolders   | yes         |                               |
| getIndexes        | no          | in progress                   |
| getMusicDirectory | yes         |                               |
| getGenres         | yes         |                               |
| getArtists        | partial     | musicFolderId not implemented |
| getArtist         | yes         |                               |
| getAlbum          | yes         |                               |
| getSong           | yes         |                               |
| getVideos         | no          | out of project scope          |
| getVideoInfo      | no          | out of project scope          |
| getArtistInfo     | no          | planned                       |
| getArtistInfo2    | no          | planned                       |
| getAlbumInfo      | no          | planned                       |
| getAlbumInfo2     | no          | planned                       |
| getSimilarSongs   | no          | not planned                   |
| getSimilarSongs2  | no          | not planned                   |
| getTopSongs       | no          | not planned                   |

</details>

<details>
  <summary>Album/song lists</summary>

| endpoint        | implemented | notes                                                                |
|-----------------|-------------|----------------------------------------------------------------------|
| getAlbumList    | partial     | not implemented: frequent type and musicFolderId parameter           |
| getAlbumList2   | partial     | uses implementation of getAlbumList => accepts same parameter values |
| getRandomSongs  | no          | planned                                                              |
| getSongsByGenre | no          | planned                                                              |
| getNowPlaying   | no          | out of project scope                                                 |
| getStarred      | no          | planned                                                              |
| getStarred2     | no          | planned                                                              |

</details>

<details>
  <summary>Searching</summary>

| endpoint | implemented | notes                                            |
|----------|-------------|--------------------------------------------------|
| search   | no          | not planned - marked as deprecated in API schema |
| search2  | no          | planned                                          |
| search3  | no          | planned                                          |

</details>

<details>
  <summary>Playlists</summary>

| endpoint       | implemented | notes                |
|----------------|-------------|----------------------|
| getPlaylists   | no          | out of current scope |
| getPlaylist    | no          | out of current scope |
| createPlaylist | no          | out of current scope |
| updatePlaylist | no          | out of current scope |
| deletePlaylist | no          | out of current scope |

</details>

<details>
  <summary>Media retrieval</summary>

| endpoint    | implemented | notes                               |
|-------------|-------------|-------------------------------------|
| stream      | partial     | optional parameters not implemented |
| download    | yes         |                                     |
| hls         | no          | not planned                         |
| getCaptions | no          | out of scope                        |
| getCoverArt | partial     | size parameter not implemented      |
| getLyrics   | no          | not planned                         |
| getAvatar   | no          | planned                             |

</details>

<details>
  <summary>Media annotation</summary>

| endpoint  | implemented | notes            |
|-----------|-------------|------------------|
| star      | no          | pending decision |
| unstar    | no          | pending decision |
| setRating | no          | pending decision |
| scrobble  | no          | pending decision |

</details>

<details>
  <summary>Sharing</summary>

| endpoint     | implemented | notes                |
|--------------|-------------|----------------------|
| getShares    | no          | out of project scope |
| createShares | no          | out of project scope |
| updateShare  | no          | out of project scope |
| deleteShare  | no          | out of project scope |

</details>

<details>
  <summary>Podcast</summary>

| endpoint               | implemented | notes                |
|------------------------|-------------|----------------------|
| getPodcasts            | no          | out of current scope |
| getNewestPodcasts      | no          | out of current scope |
| refreshPodcasts        | no          | out of current scope |
| createPodcastChannel   | no          | out of current scope |
| deletePodcastChannel   | no          | out of current scope |
| deletePodcastEpisode   | no          | out of current scope |
| downloadPodcastEpisode | no          | out of current scope |

</details>

<details>
  <summary>Jukebox</summary>

| endpoint       | implemented | notes                       |
|----------------|-------------|-----------------------------|
| jukeboxControl | no          | no such feature in Jellyfin |

</details>

<details>
  <summary>System</summary>

| endpoint                   | implemented | notes                       |
|----------------------------|-------------|-----------------------------|
| getInternetRadioStations   | no          | no such feature in Jellyfin |
| createInternetRadioStation | no          | no such feature in Jellyfin |
| updateInternetRadioStation | no          | no such feature in Jellyfin |
| deleteInternetRadioStation | no          | no such feature in Jellyfin |

</details>

<details>
  <summary>Chat</summary>

| endpoint        | implemented | notes                       |
|-----------------|-------------|-----------------------------|
| getChatMessages | no          | no such feature in Jellyfin |
| addChatMessage  | no          | no such feature in Jellyfin |

</details>

<details>
  <summary>User management</summary>

| endpoint       | implemented | notes                |
|----------------|-------------|----------------------|
| getUser        | no          | planned              |
| getUsers       | no          | out of project scope |
| createUser     | no          | out of project scope |
| updateUser     | no          | out of project scope |
| deleteUser     | no          | out of project scope |
| changePassword | no          | out of project scope |

</details>

<details>
  <summary>Bookmarks</summary>

| endpoint       | implemented | notes            |
|----------------|-------------|------------------|
| getBookmarks   | no          | pending decision |
| createBookmark | no          | pending decision |
| deleteBookmark | no          | pending decision |
| getPlayQueue   | no          | pending decision |
| savePlayQueue  | no          | pending decision |

</details>

<details>
  <summary>Media library scanning</summary>

| endpoint      | implemented | notes                |
|---------------|-------------|----------------------|
| getScanStatus | no          | out of project scope |
| startScan     | no          | out of project scope |

</details>

# Installation

The plugin can be installed either via repository or [manually](#manual-build-and-installation)

## Repo Install

Jellyfin 10.6.0 introduces 3rd party plugin repositories (see: [announcement](https://jellyfin.org/posts/plugin-updates/)), configure the following to follow stable builds for this plugin

- Repo name: JellySonic (or whatever, can be anything)
- Repo URL: `https://raw.githubusercontent.com/lyarenei/JellySonic/master/manifest.json`

After you add the repository, you should be able to see a JellySonic plugin in the catalog.
Install your preferred version and restart the server as asked.

## Configuration

TBD

# Manual build and installation

.NET 6.0 is required to build the JellySonic plugin.
To install the .NET SDK on Linux or macOS, see the download page at https://dotnet.microsoft.com/download.
Native package manager instructions can be found for Debian, RHEL, Ubuntu, Fedora, SLES, and CentOS.

Once the SDK is installed, run the following.

```
git clone https://github.com/lyarenei/JellySonic
cd JellySonic
dotnet publish -c Release
```

If the build is successful, the compiler will report the path to your Plugin dll (`JellySonic/bin/Release/net6.0/JellySonic.dll`)

Copy the plugin DLL file into your Jellyfin ${CONFIG_DIR}/plugins/JellySonic directory.
Create the JellySonic directory if it does not exist, and make sure Jellyfin can access it.

# Running Jellyfin server

See instructions on the [offical website](https://jellyfin.org/downloads/).
