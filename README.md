# JellySonic
An **experimental** subsonic plugin for Jellyfin.

This plugin enables your Jellyfin server to serve Subsonic clients.

## Features
- Implemented endpoints:
  - download
  - getArtist
  - getArtists
  - getAlbum
  - getCoverArt
    - size parameter is not implemented
  - getLicense
  - getMusicDirectory
  - getMusicFolders
  - getSong
  - ping
    - does not require authentication
      - TBD configurable (per user)
  - stream
    - no optional parameters implemented

### Known limitations/bugs

- Cover arts are not implemented.
- Song
  - content type is not implemented
  - parent attribute is not implemented
  - artist ID is not implemented

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
