using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

    // /// <summary>
    // /// Get users.
    // /// </summary>
    // /// <returns>Users list.</returns>
    // public Collection<JellySonicUser> GetUsers()
    // {
    //     return new Collection<JellySonicUser>(_users);
    // }
    //
    // /// <summary>
    // /// Set users.
    // /// </summary>
    // /// <param name="users">Users collection.</param>
    // public void SetUsers(IEnumerable<JellySonicUser> users)
    // {
    //     _users = users.ToList();
    // }
}
