# JellySonic
An **experimental** subsonic plugin for Jellyfin.

This plugin enables your Jellyfin server to serve Subsonic clients.

### Implemented Endpoints
See [here](doc/endpoints.md).

### Subsonic authentication
As the Subsonic authentication mechanism is pretty much insecure (the password is secured in transit at best),
this plugin does not use Jellyfin user passwords to authorize Subsonic requests.
This is because either way the password is available in plaintext:
- When using token auth, the server needs an access to the password in order to generate the token for comparison
- When using legacy auth, the password is sent with the request (optionally obfuscated in hex)


Because of that the plugin explicitly requires administrator to set a Subsonic password for their users,
so they can authenticate with the server. This password must be stored in plaintext, as described above. Please keep that in mind.

To configure the user, refer to [config](#configuration) section below.
Although there are no password requirements enforced, the password should not be the same as the Jellyfin user password.

## Installation

The plugin can be installed either via repository or [manually](#manual-build-and-installation).

After installation, do not forget to **configure** the plugin as mentioned in the note above.

### Repo Install

- Repo name: JellySonic (or whatever, can be anything)
- Repo URL: `https://raw.githubusercontent.com/lyarenei/JellySonic/master/manifest.json`

After you add the repository, you should be able to see a JellySonic plugin in the catalog.
Install your preferred version and restart the server as asked.

### Configuration

To configure a user:
1. Navigate to plugin settings
2. Select the user you want to configure
3. Set a password for authentication

The password has no validation or anything.
The only requirement is that it should not be the same as the user's Jellyfin password.

After saving the configuration, the user should be able to authenticate with the password set in the plugin configuration.
The user can use both token and password method, although the token method is preferred.

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
