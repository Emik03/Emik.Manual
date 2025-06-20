// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents an element in the <c>regions.json</c> file.</summary>
/// <param name="Name">Name of the region.</param>
/// <param name="Logic">
/// The boolean logic object that describes the required items, counts, etc. needed to reach this region.
/// </param>
/// <param name="ConnectsTo">
/// A list of other regions that the player can reach from this region. Only describe forward connections
/// with this setting, as backward connections are implied from regions you have already accessed.
/// </param>
/// <param name="IsStarting">
/// Is this region accessible from the start? Or, does this region not require a connection from another region first?
/// </param>
/// <param name="Entrances">
/// Additional requirements to use a specific connection into this region.
/// This is the same as <see cref="Exits"/>, but from the other side.
/// </param>
/// <param name="Exits">Additional requirements to use a specific connection out of this region.</param>
/// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/regions.md"/>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct Region(
    Chars Name,
    Logic? Logic = null,
    ImmutableArray<Region> ConnectsTo = default,
    bool IsStarting = false,
    ImmutableArray<Passage> Entrances = default,
    ImmutableArray<Passage> Exits = default
) : IAddTo, IArchipelago<Region>
{
    /// <inheritdoc />
    [Pure]
    public static implicit operator Region(string? name) => new(name.AsMemory());

    /// <inheritdoc />
    [Pure]
    public static implicit operator Region(ReadOnlyMemory<char> name) => new(name);

    /// <inheritdoc />
    void IAddTo.CopyTo([NotNullIfNotNull(nameof(value))] ref JsonNode? value)
    {
        JsonObject obj = [];

        if (IsStarting)
            obj["starting"] = true;

        if (Logic is not null)
            obj["requires"] = Logic.ToString();

        if (!ConnectsTo.IsDefaultOrEmpty)
            obj["connects_to"] = IArchipelago<Region>.Json(ConnectsTo);

        if (!Exits.IsDefaultOrEmpty)
            obj["exit_requires"] = Json(Exits);

        if (!Entrances.IsDefaultOrEmpty)
            obj["entrance_requires"] = Json(Entrances);

        (value ??= new JsonObject())[Name.ToString()] = obj;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => IAddTo.ToJsonString(this);

    /// <inheritdoc cref="IArchipelago{T}.Json(ImmutableArray{T})"/>
    [Pure]
    static JsonObject Json(ImmutableArray<Passage> span)
    {
        JsonObject ret = [];

        foreach (var (region, logic) in span)
            if (logic is not null)
                ret[region.Name.ToString()] = logic.ToString();

        return ret;
    }
}
