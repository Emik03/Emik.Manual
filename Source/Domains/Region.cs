// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

using Accumulator = (HashSet<string> Visited, Region Current, Logic? Logic, bool Found);

/// <summary>Represents an element in the <c>regions.json</c> file.</summary>
/// <param name="Name">Name of the region.</param>
/// <param name="SelfLogic">
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
    Logic? SelfLogic = null,
    ImmutableArray<Region> ConnectsTo = default,
    bool IsStarting = false,
    ImmutableArray<Passage> Entrances = default,
    ImmutableArray<Passage> Exits = default
) : IAddTo, IArchipelago<Region>, ILogicNode<Region>, IEqualityOperators<Region, Region, bool>, IEquatable<object>
{
    /// <summary>Gets itself as a <see cref="Manual.Logic"/> requirement.</summary>
    [Pure]
    public Logic Logic => new(this);

    /// <inheritdoc />
    [Pure]
    public static implicit operator Region(string? name) => new(name.AsMemory());

    /// <inheritdoc />
    [Pure]
    public static implicit operator Region(ReadOnlyMemory<char> name) => new(name);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(in Region left, in Region right) => new Logic(left) & new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(in Region left, Logic? right) => new Logic(left) & right;

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(Logic? left, in Region right) => left & new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(in Region left, in Region right) => new Logic(left) | new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(in Region left, Logic? right) => new Logic(left) | right;

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(Logic? left, in Region right) => left | new Logic(right);

    /// <inheritdoc />
    void IAddTo.CopyTo(
        [NotNullIfNotNull(nameof(value))] ref JsonNode? value,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    )
    {
        JsonNode obj = new JsonObject();

        if (IsStarting)
            obj["starting"] = true;

        if (!ConnectsTo.IsDefaultOrEmpty)
            obj["connects_to"] = IArchipelago<Region>.Json(ConnectsTo);

        if (!Exits.IsDefaultOrEmpty)
            obj["exit_requires"] = Json(Exits, locations, regions);

        if (!Entrances.IsDefaultOrEmpty)
            obj["entrance_requires"] = Json(Entrances, locations, regions);

        (value ??= new JsonObject())[Name.ToString()] = obj;
        SelfLogic?.CopyTo(ref obj, locations, regions);
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Name.ToString();

    /// <summary>Gets the combined logic.</summary>
    /// <param name="regions">All regions.</param>
    /// <returns>The combined logic.</returns>
    public Logic? ExpandedLogic(IReadOnlyCollection<Region>? regions) =>
        regions is null || IsStarting ? SelfLogic : regions.Aggregate((regions, Logic: (Logic?)null), Start).Logic;

    /// <summary>Gets the starting regions, excluding itself.</summary>
    /// <param name="regions">All regions.</param>
    /// <returns>The starting regions, excluding itself.</returns>
    [Pure]
    HashSet<string> Starters(IEnumerable<Region> regions)
    {
        var name = Name;

        return regions.Where(x => x.IsStarting && x.Name != name)
           .Select(x => x.Name.ToString())
           .ToSet(StringComparer.Ordinal);
    }

    /// <inheritdoc cref="IArchipelago{T}.Json(ImmutableArray{T})"/>
    [Pure]
    static JsonObject Json(
        ImmutableArray<Passage> span,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    )
    {
        JsonObject ret = [];

        foreach (var (region, logic) in span)
            if (logic?.Expanded(locations, regions) is { } l)
                ret[region.Name.ToString()] = l.ToString();

        return ret;
    }

    /// <summary>Combines the logic.</summary>
    /// <param name="a">The accumulator.</param>
    /// <param name="connection">The next connection.</param>
    /// <returns>The new accumulator.</returns>
    [Pure]
    Accumulator Combine(Accumulator a, Region connection) =>
        a is var (visited, current, logic, _) &&
        Next(new(visited, StringComparer.Ordinal), connection) is (var innerLogic, true) &&
        (innerLogic &
            current.Exits.FirstOrDefault(x => x.Name == connection.Name)?.SelfLogic &
            connection.Entrances.FirstOrDefault(x => x.Name == current.Name)?.SelfLogic) is var and
            ? a with { Logic = logic | and, Found = true }
            : a;

    /// <summary>Starts the recursive search.</summary>
    /// <param name="a">The accumulator.</param>
    /// <param name="region">The potential starting region to start the search in.</param>
    /// <returns>The next accumulator.</returns>
    [Pure]
    (IReadOnlyCollection<Region>, Logic?) Start((IReadOnlyCollection<Region> Regions, Logic? Logic) a, Region region) =>
        region.IsStarting && Next(region.Starters(a.Regions), region) is (var l, true)
            ? a with { Logic = a.Logic | l }
            : a;

    /// <summary>Performs the next step.</summary>
    /// <param name="visited">The names of the visited regions.</param>
    /// <param name="current">The current region to look at.</param>
    /// <returns>The logic to get to this area, and whether it is relevant.</returns>
    [Pure]
    (Logic?, bool) Next(HashSet<string> visited, Region current) =>
        !visited.GetAlternateLookup<ReadOnlySpan<char>>().Add(current.Name.Span) ? default :
        Name == current.Name ? (current.SelfLogic, true) :
        current.ConnectsTo is { IsDefaultOrEmpty: false } connections &&
        connections.Aggregate((visited, current, (Logic?)null, false), Combine) is var (_, _, l, f) ? (l, f) :
        default;
}
