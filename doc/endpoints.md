# Endpoints

The state of endpoints implementation can be found in the tables below. If you have found any issues, please check the
implementation state before opening an issue.

The current scope is to implement such endpoints to allow clients to provide basic/minimal, but reasonable user
experience.

The primary aim of this plugin is to serve the subsonic clients for _music_ playback.

### System

| endpoint   | implemented | notes |
|------------|-------------|-------|
| ping       | yes         |       |
| getLicense | yes         |       |

### Browsing

Videos are not supported by this plugin.

Top and similar songs are not likely to be implemented.

| endpoint          | implemented | notes              |
|-------------------|-------------|--------------------|
| getMusicFolders   | yes         |                    |
| getIndexes        | yes         |                    |
| getMusicDirectory | yes         |                    |
| getGenres         | yes         |                    |
| getArtists        | yes         |                    |
| getArtist         | yes         |                    |
| getAlbum          | yes         |                    |
| getSong           | yes         |                    |
| getVideos         | no          |                    |
| getVideoInfo      | no          |                    |
| getArtistInfo     | partial     | returns empty data |
| getArtistInfo2    | partial     | returns empty data |
| getAlbumInfo      | no          |                    |
| getAlbumInfo2     | no          |                    |
| getSimilarSongs   | no          |                    |
| getSimilarSongs2  | no          |                    |
| getTopSongs       | no          |                    |

### Album/song lists

Now playing is not likely to be implemented.

| endpoint        | implemented | notes                              |
|-----------------|-------------|------------------------------------|
| getAlbumList    | partial     | frequent list type not implemented |
| getAlbumList2   | partial     | same as getAlbumList               |
| getRandomSongs  | no          |                                    |
| getSongsByGenre | no          |                                    |
| getNowPlaying   | no          |                                    |
| getStarred      | no          |                                    |
| getStarred2     | no          |                                    |

### Searching

Search (1) should not be implemented as is marked as deprecated in API schema.

| endpoint | implemented | notes |
|----------|-------------|-------|
| search   | no          |       |
| search2  | yes         |       |
| search3  | yes         |       |

### Playlists

| endpoint       | implemented | notes |
|----------------|-------------|-------|
| getPlaylists   | no          |       |
| getPlaylist    | no          |       |
| createPlaylist | no          |       |
| updatePlaylist | no          |       |
| deletePlaylist | no          |       |

### Media retrieval

Transcoding is not supported - at least for now. There does not seem to be a way to start transcode job from within a
plugin.

Captions are only for video content, which is not supported by this plugin.

Jellyfin does not store lyrics, this endpoint won't be implemented.

| endpoint    | implemented | notes                               |
|-------------|-------------|-------------------------------------|
| stream      | partial     | optional parameters not implemented |
| download    | yes         |                                     |
| hls         | no          |                                     |
| getCaptions | no          |                                     |
| getCoverArt | partial     | size parameter not implemented      |
| getLyrics   | no          |                                     |
| getAvatar   | no          |                                     |

### Media annotation

Scrobbling uses Jellyfin's internal `OnPlaybackStarted/Stopped` API, which correctly displays activity on admin dashboard.
This also allows administrators to use additional plugins for submitting activity to external services.
Unfortunately, there doesn't seem a way to set playback with custom time,
so optional `time` parameter is ignored => scrobbles are only in current time.

| endpoint  | implemented | notes                          |
|-----------|-------------|--------------------------------|
| star      | yes         |                                |
| unstar    | yes         |                                |
| setRating | no          |                                |
| scrobble  | partial     | time parameter not implemented |

### Sharing

Although sharing is possible in Jellyfin, these will likely not be implemented.

| endpoint     | implemented | notes |
|--------------|-------------|-------|
| getShares    | no          |       |
| createShares | no          |       |
| updateShare  | no          |       |
| deleteShare  | no          |       |

### Podcast

May be implemented in the future.

| endpoint               | implemented | notes |
|------------------------|-------------|-------|
| getPodcasts            | no          |       |
| getNewestPodcasts      | no          |       |
| refreshPodcasts        | no          |       |
| createPodcastChannel   | no          |       |
| deletePodcastChannel   | no          |       |
| deletePodcastEpisode   | no          |       |
| downloadPodcastEpisode | no          |       |

### Jukebox

No such feature in Jellyfin.

| endpoint       | implemented | notes |
|----------------|-------------|-------|
| jukeboxControl | no          |       |

### System

No such feature in Jellyfin.

| endpoint                   | implemented | notes |
|----------------------------|-------------|-------|
| getInternetRadioStations   | no          |       |
| createInternetRadioStation | no          |       |
| updateInternetRadioStation | no          |       |
| deleteInternetRadioStation | no          |       |

### Chat

No such feature in Jellyfin.

| endpoint        | implemented | notes |
|-----------------|-------------|-------|
| getChatMessages | no          |       |
| addChatMessage  | no          |       |

### User management

User management should be the responsibility of the Jellyfin server and so these endpoints should not be implemented.

| endpoint       | implemented | notes                             |
|----------------|-------------|-----------------------------------|
| getUser        | partial     | folders in the response are empty |
| getUsers       | no          |                                   |
| createUser     | no          |                                   |
| updateUser     | no          |                                   |
| deleteUser     | no          |                                   |
| changePassword | no          |                                   |

### Bookmarks

May be implemented in the future.

| endpoint       | implemented | notes |
|----------------|-------------|-------|
| getBookmarks   | no          |       |
| createBookmark | no          |       |
| deleteBookmark | no          |       |
| getPlayQueue   | no          |       |
| savePlayQueue  | no          |       |

### Media library scanning

These endpoints may be implemented in the future, although library management should be the responsibility of the
Jellyfin server.

| endpoint      | implemented | notes |
|---------------|-------------|-------|
| getScanStatus | no          |       |
| startScan     | no          |       |
