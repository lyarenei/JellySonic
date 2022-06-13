# JellySonic
An **experimental** subsonic plugin for Jellyfin.

This plugin enables your Jellyfin server to serve Subsonic clients.

### Implemented Endpoints
See [here](doc/endpoints.md).

### Subsonic authentication
As the Subsonic authentication mechanism is pretty much insecure (the password is secured in transit at best),
this plugin does not use Jellyfin user credentials for authorization.
This is because either way the password would be available in plaintext:
- When using token auth, the server needs an access to the password in order to generate the token for comparison
- When using legacy auth, the password is sent with the request (optionally obfuscated in hex)


To avoid such leaks, the plugin uses API keys in for authentication instead.
The configuration is described in the [config](#configuration) section below.

## Installation

The plugin can be installed either via repository or [manually](#manual-build-and-installation).

After installation, do not forget to **configure** the plugin as mentioned in the note above.

### Repo Install

- Repo name: JellySonic (or whatever, can be anything)
- Repo URL: `https://raw.githubusercontent.com/lyarenei/JellySonic/master/manifest.json`

After you add the repository, you should be able to see a JellySonic plugin in the catalog.
Install your preferred version and restart the server as asked.

### Configuration

Note: There are currently no options for user configuration.

To configure a user:
1. Navigate to plugin settings
2. Select the user you want to configure
3. ???

#### Configuring authentication

The plugin uses a combination of the API key and client name.
However, as the plugin still needs to work with a Jellyfin user for i.e.: accessing libraries,
the plugin requires the App name for the API token in format `<jellyfin username>-<identifier>`.
The identifier value is used for differentiation between multiple API keys for the same user
and can be any non-empty string. The identifier can also contain hyphens.

Example of a correct App name: `johndoe-subsonic_client`
- this matches to a Jellyfin username `johndoe`
- and the identifier is `subsonic_client`

To set up an API key, navigate to API keys and add a new API key, following the App name guideline described above.
The subsonic client can use both token and password authentication methods, although the token method is preferred.

# Manual build and installation

.NET 6.0 is required to build the JellySonic plugin.
To install the .NET SDK on Linux or macOS, check out the download page at https://dotnet.microsoft.com/download.
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

If you don't have a Jellyfin server yet, check out the instructions on the [offical website](https://jellyfin.org/downloads/).
