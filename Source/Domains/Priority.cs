// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Indicates the importance and general placement of the <see cref="Item"/>.</summary>
/// <seealso href="https://github.com/ArchipelagoMW/Archipelago/blob/main/docs/world%20api.md#items"/>
[Flags]
public enum Priority : byte
{
    /// <summary>No flags, which gets coerced to <see cref="Filler"/>.</summary>
    None,

    /// <summary>Negative impact on the player.</summary>
    Trap,

    /// <summary>A regular item or trash item.</summary>
    Filler,

    /// <summary>
    /// Item that is especially useful. Cannot be placed on excluded or unreachable locations. When combined with
    /// another flag like <see cref="Progression"/>, it means "an especially useful progression item".
    /// </summary>
    Useful = 1 << 2,

    /// <summary>
    /// Items which a player may require to progress in their world. If an item can possibly be
    /// considered for logic (it's referenced in a location's rules) it must be progression.
    /// </summary>
    Progression = 1 << 3,

    /// <summary>An especially useful progression item.</summary>
    ProgressionUseful = Progression | Useful,

    /// <summary>Denotes that an item should not be moved to an earlier sphere for the purpose of balancing.</summary>
    SkipBalancing = 1 << 4,

    /// <summary>
    /// The combination of <see cref="Progression"/> and <see cref="SkipBalancing"/>, i.e., a progression item that will
    /// not be moved around by progression balancing; used, e.g., for currency or tokens, to not flood early spheres.
    /// </summary>
    ProgressionSkipBalancing = Progression | SkipBalancing,

    /// <summary>An especially useful progression item that will not be moved around by progression balancing.</summary>
    ProgressionUsefulSkipBalancing = Progression | Useful | SkipBalancing,

    /// <summary>Are all copies of this item supposed to be only in your locations, or can they be anywhere?</summary>
    Local = 1 << 5,
}
