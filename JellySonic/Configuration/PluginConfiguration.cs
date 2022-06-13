using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JellySonic.Models;
using MediaBrowser.Model.Plugins;

namespace JellySonic.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        Users = new List<JellySonicUser>();
    }

    /// <summary>
    /// Gets or sets users.
    /// </summary>
    [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "I don't know how to properly write models for serialization")]
    [SuppressMessage("Design", "CA1002:Do not expose generic lists", Justification = "I don't know how to properly write models for serialization")]
    public List<JellySonicUser> Users { get; set; }
}
