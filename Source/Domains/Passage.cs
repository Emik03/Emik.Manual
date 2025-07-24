// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Describes the entry when connecting regions conditionally with logic.</summary>
/// <param name="Region">The region to connect to.</param>
/// <param name="SelfLogic">The logic that would enable this connection.</param>
public sealed partial record Passage(Region Region, Logic? SelfLogic = null) : IAddTo,
    IArchipelago<Passage>,
    ILogicNode<Passage>,
    IEqualityOperators<Passage, Passage, bool>,
    IEquatable<object>
{
    /// <inheritdoc />
    public Chars Name => Region.Name;

    /// <inheritdoc />
    public Logic Logic => Region & SelfLogic;

    /// <inheritdoc />
    public static implicit operator Passage(string? name) => new(name);

    /// <inheritdoc />
    public static implicit operator Passage(ReadOnlyMemory<char> name) => new(name);

    /// <summary>Implicitly invokes <see cref="Domains.Passage(Domains.Region, Manual.Logic)"/>.</summary>
    /// <param name="region">The region.</param>
    /// <returns>The new <see cref="Manual.Logic"/> instance.</returns>
    public static implicit operator Passage(Region region) => new(region);

    /// <summary>Implicitly invokes <see cref="Domains.Passage(Domains.Region, Manual.Logic)"/>.</summary>
    /// <param name="selfLogic">The region.</param>
    /// <returns>The new <see cref="Manual.Logic"/> instance.</returns>
    public static implicit operator Passage((Region Region, Logic? SelfLogic) selfLogic) =>
        new(selfLogic.Region, selfLogic.SelfLogic);

    /// <summary>Implicitly invokes <see cref="Domains.Passage(Domains.Region, Manual.Logic)"/>.</summary>
    /// <param name="selfLogic">The region.</param>
    /// <returns>The new <see cref="Manual.Logic"/> instance.</returns>
    public static implicit operator Passage((Logic? SelfLogic, Region Region) selfLogic) =>
        new(selfLogic.Region, selfLogic.SelfLogic);

    /// <inheritdoc />
    public static Logic operator &(Logic? left, in Passage right) => left & new Logic(right);

    /// <inheritdoc />
    public static Logic operator &(in Passage left, Logic? right) => new Logic(left) & right;

    /// <inheritdoc />
    public static Logic operator &(in Passage left, in Passage right) => new Logic(left) & new Logic(right);

    /// <inheritdoc />
    public static Logic operator |(Logic? left, in Passage right) => left | new Logic(right);

    /// <inheritdoc />
    public static Logic operator |(in Passage left, Logic? right) => new Logic(left) | right;

    /// <inheritdoc />
    public static Logic operator |(in Passage left, in Passage right) => new Logic(left) | new Logic(right);

    /// <inheritdoc />
    public void CopyTo(
        [NotNullIfNotNull(nameof(value))] ref JsonNode? value,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    )
    {
        if (SelfLogic is not null)
            (value ??= new JsonObject())[Name.ToString()] =
                (SelfLogic.ExpandLocations(locations, regions) ?? SelfLogic).ToString();
    }

    /// <inheritdoc />
    public override string ToString() => $"{Region}: {SelfLogic}";
}
