// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;
#pragma warning disable EAM002
/// <summary>Represents the element in the <c>items.json</c> file.</summary>
/// <param name="Name">The name of the item.</param>
/// <param name="Priority">The importance and placement bias of the item.</param>
/// <param name="Categories">A list of categories to be applied to this item.</param>
/// <param name="Count">Total number of this item that will be in the item pool for randomization.</param>
/// <param name="GiveItems">
/// A dictionary of values this item has. Used for <see cref="Logic.Builtin.ItemValue"/>.
/// </param>
/// <param name="Early">
/// How many copies of this item are required to be placed somewhere accessible from the start.
/// </param>
/// <param name="LocalEarly"></param>
/// <param name="Id"></param>
/// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/items.md"/>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct Item(
    Chars Name,
    Priority Priority = Priority.Progression,
    ImmutableArray<Category> Categories = default,
    int Count = 1,
    ImmutableArray<(Chars PhantomItemName, int Amount)> GiveItems = default,
    int Early = 0,
    int LocalEarly = 0,
    int? Id = null
) : IAddTo, IArchipelago<Item>, IEqualityOperators<Item, Item, bool>, IEquatable<object>
{
    /// <summary>Makes a requirement that the item should be obtained multiple times.</summary>
    /// <param name="count">The count to times to fulfill the requirement.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The parameter <paramref name="count"/> is negative, or greater than <see cref="Count"/>.
    /// </exception>
    [Pure]
    public Logic? this[int count] =>
        count is 0 ? null :
        (uint)count <= (uint)Count ? new Logic(this, count) :
        throw new ArgumentOutOfRangeException(nameof(count), count, $"Cannot have {count} amounts of {Name}.");

    /// <summary>Makes a requirement that all of this item should be obtained.</summary>
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

    /// <summary>Makes a requirement that half of this item should be obtained.</summary>
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
    public static implicit operator Item([Match(World.AllowedItemChars, true)] string? name) => new(name.AsMemory());

    /// <inheritdoc />
    [Pure]
    public static implicit operator Item([Match(World.AllowedItemChars, true)] ReadOnlyMemory<char> name) => new(name);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(Item left, Item right) => new Logic(left) & new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(Item left, Logic? right) => new Logic(left) & right;

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static Logic operator &(Logic? left, Item right) => left & new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(Item left, Item right) => new Logic(left) | new Logic(right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(Item left, Logic? right) => new Logic(left) | right;

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static Logic operator |(Logic? left, Item right) => left | new Logic(right);

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
    void IAddTo.CopyTo(
        [NotNullIfNotNull(nameof(value))] ref JsonNode? value,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    )
    {
        JsonObject obj = new() { ["name"] = ToString() };

        if (Count > 1)
            obj["count"] = Count;

        if (Id is { } id)
            obj["id"] = id;

        if (Early is not 0)
            obj["early"] = Early;

        if (LocalEarly is not 0)
            obj["local_early"] = LocalEarly;

        if (!Categories.IsDefaultOrEmpty)
            obj["category"] = IArchipelago<Category>.Json(Categories);

        if (Priority.Has(Priority.Trap))
            obj["trap"] = true;

        if (Priority.Has(Priority.Local))
            obj["local"] = true;

        if (Priority.Has(Priority.Filler))
            obj["filler"] = true;

        if (Priority.Has(Priority.Useful))
            obj["useful"] = true;

        if (Priority.Has(Priority.Progression))
            obj[Priority.Has(Priority.SkipBalancing) ? "progression_skip_balancing" : "progression"] = true;
        else if (Priority.Has(Priority.SkipBalancing))
            obj["skip_balancing"] = true;

        if (!GiveItems.IsDefaultOrEmpty)
        {
            JsonObject dictionary = [];

            foreach (var (c, v) in GiveItems)
            {
                var k = c.ToString();
                dictionary[k] = v + (dictionary.TryGetPropertyValue(k, out var node) ? node?.GetValue<int>() ?? 0 : 0);
            }

            obj["value"] = dictionary;
        }

        switch (value)
        {
            case JsonObject:
                value = obj;
                return;
            case JsonArray it:
                it.Add(obj);
                break;
            default:
                value = new JsonArray(obj);
                break;
        }
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Name.ToString();

    /// <summary>Makes a requirement that the item should be obtained in at least some threshold percentage.</summary>
    /// <param name="percent">The percent.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public Logic? Percent(int percent) => Logic.Percent(this, percent);

    /// <summary>Makes a requirement that the item should be obtained in at least some threshold percentage.</summary>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public Logic? Percent(double index, double scaling) => Logic.Percent(this, index, scaling);
}
