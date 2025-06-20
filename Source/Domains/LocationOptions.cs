// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents the boolean options for <see cref="Location"/>.</summary>
[Flags]
public enum LocationOptions : byte
{
    /// <summary>No options.</summary>
    None,

    /// <summary>Getting this <see cref="Location"/> will mark the game as Goal.</summary>
    Victory,

    /// <summary>This <see cref="Location"/> will be hinted immediately, revealing what it contains.</summary>
    PreHint,
}
