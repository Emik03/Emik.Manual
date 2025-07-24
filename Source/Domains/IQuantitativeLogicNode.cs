// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Contains quantities to construct <see cref="Logic"/>.</summary>
public interface IQuantitativeLogicNode
{
    /// <summary>Makes a requirement that this instance should be obtained a specific amount of times.</summary>
    /// <param name="count">The count to times to fulfill the requirement.</param>
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="count"/> is negative.</exception>
    [Pure]
    public Logic? this[int count] { get; }

    /// <summary>Makes a requirement that all of this instance should be obtained.</summary>
    [Pure]
    public Logic All { get; }

    /// <summary>Makes a requirement that half of this instance should be obtained.</summary>
    [Pure]
    public Logic Half { get; }

    /// <summary>
    /// Makes a requirement that items with this instance should be obtained in at least some threshold percentage.
    /// </summary>
    /// <param name="percent">The percent.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public Logic? Percent(int percent);

    /// <summary>
    /// Makes a requirement that items with this instance should be obtained in at least some threshold percentage.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public Logic? Percent(double index, double scaling);
}
