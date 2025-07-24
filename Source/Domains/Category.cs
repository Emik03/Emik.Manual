// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Allows for grouping of checks.</summary>
/// <param name="Name">The name of the category.</param>
/// <param name="IsHidden">Whether to hide this category in the GUI.</param>
/// <param name="Yaml">If populated, any yaml options that needs to be true for this category to exist.</param>
/// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/categories-for-items-and-locations.md"/>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct Category(Chars Name, bool IsHidden = false, params ImmutableArray<Yaml> Yaml)
    : IAddTo, IArchipelago<Category>, IEqualityOperators<Category, Category, bool>, IEquatable<object>
{
    /// <summary>Makes a requirement that the category should be obtained multiple times.</summary>
    /// <param name="count">The count to times to fulfill the requirement.</param>
    /// <exception cref="ArgumentOutOfRangeException">The parameter <paramref name="count"/> is negative.</exception>
    [Pure]
    public Logic? this[int count] =>
        count is 0 ? null :
        count > 0 ? new Logic(this, count) :
        throw new ArgumentOutOfRangeException(nameof(count), count, $"Cannot have {count} amounts of {Name}.");

    /// <summary>Makes a requirement that all items with this category should be obtained.</summary>
    [Pure]
    public Logic All
    {
        get
        {
            var logic = Logic.Percent(this, 100);
            Debug.Assert(logic is not null); // We pass a percent that is greater than zero.
            return logic;
        }
    }

    /// <summary>Makes a requirement that half items with this category should be obtained.</summary>
    [Pure]
    public Logic Half
    {
        get
        {
            var logic = Logic.Percent(this, 50);
            Debug.Assert(logic is not null); // We pass a percent that is greater than zero.
            return logic;
        }
    }

    /// <inheritdoc />
    [Pure]
    public static implicit operator Category(string? name) => new(name.AsMemory());

    /// <inheritdoc />
    [Pure]
    public static implicit operator Category(ReadOnlyMemory<char> name) => new(name);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(in Category left, in Category right) => new Logic(left) & new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(in Category left, Logic? right) => new Logic(left) & right;

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(Logic? left, in Category right) => left & new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(in Category left, in Category right) => new Logic(left) | new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(in Category left, Logic? right) => new Logic(left) | right;

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(Logic? left, in Category right) => left | new Logic(right);

    /// <inheritdoc />
    void IAddTo.CopyTo(
        [NotNullIfNotNull(nameof(value))] ref JsonNode? value,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    )
    {
        if (!IsHidden && Yaml.IsDefaultOrEmpty)
            return;

        var category = new JsonObject();

        if (IsHidden)
            category["hidden"] = true;

        if (Manual.Yaml.Json(Yaml) is { } options)
            category["yaml_option"] = options;

        (value ??= new JsonObject())[ToString()] = category;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Name.ToString();

    /// <summary>
    /// Makes a requirement that items with this category should be obtained in at least some threshold percentage.
    /// </summary>
    /// <param name="percent">The percent.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public Logic? Percent(int percent) => Logic.Percent(this, percent);

    /// <summary>
    /// Makes a requirement that items with this category should be obtained in at least some threshold percentage.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public Logic? Percent(double index, double scaling) => Logic.Percent(this, index, scaling);
}
