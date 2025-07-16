// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;
#pragma warning disable CS9113
/// <summary>
/// Represents the set of requirements for <see cref="Location"/> or <see cref="Region"/> to be considered reachable.
/// </summary>
[Choice] // ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class Logic(
    Item item,
    (Item Item, int Count) itemCount,
    (Item Item, Logic.Explicit<int> Percent) itemPercent,
    Category category,
    (Category Category, int Count) categoryCount,
    (Category Category, Logic.Explicit<int> Percent) categoryPercent,
    (Logic? Left, Logic? Right) or,
    (Logic? Left, Logic? Right) and,
    Location location,
    (Chars PhantomItem, Logic.Explicit<int> Count) itemValue,
    (Item Item, ValueTuple Marker) optOne,
    Logic? optAll,
    Yaml enabled,
    Yaml disabled,
    (Yaml Yaml, ExpressionType Comparator, int Count) yamlCompare,
    (Chars Name, Chars Args) custom
) : IAddTo
{
    /// <summary>Encapsulates a value to restrict implicit conversions.</summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="Value">The value.</param>
    [StructLayout(LayoutKind.Auto)]
    public readonly partial record struct Explicit<T>(T Value)
    {
        /// <inheritdoc />
        [Pure]
        public override string ToString() => Value?.ToString() ?? "";
    }

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    public Logic(string item)
        : this((Item)item) { }

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    public Logic(ReadOnlyMemory<char> item)
        : this((Item)item) { }

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="count">The count.</param>
    /// <param name="item">The item.</param>
    public Logic(int count, string item)
        : this(ValueTuple.Create((Item)item, count)) { }

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    /// <param name="count">The count.</param>
    public Logic(string item, int count)
        : this(ValueTuple.Create((Item)item, count)) { }

    /// <summary>Determines whether this node is optimized.</summary>
    public bool IsOptimized { get; internal set; }

    /// <summary>Gets the binary node.</summary>
    // ReSharper disable once ConvertToAutoProperty ConvertToAutoPropertyWhenPossible
    internal (Logic? Left, Logic? Right) Binary => _or;

    /// <summary>Gets the builtin kind.</summary>
    public Builtin? BuiltinKind =>
        Map<Builtin?>(
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => Builtin.CanReachLocation,
            _ => Builtin.ItemValue,
            _ => Builtin.OptOne,
            _ => Builtin.OptAll,
            _ => Builtin.YamlEnabled,
            _ => Builtin.YamlDisabled,
            _ => Builtin.YamlCompare,
            _ => Builtin.Custom
        );

    /// <summary>
    /// Gets the yaml settings that this <see cref="Logic"/> references. This value is <see langword="null"/> if there
    /// are no yaml settings defined within this logic. Therefore, this collection can never be empty.
    /// </summary>
    [Pure]
    public IEnumerable<(Builtin Context, Yaml Yaml)>? YamlSettings =>
        Map<IEnumerable<(Builtin Context, Yaml Yaml)>?>(
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            _ => null,
            x => x.Left?.YamlSettings is { } left
                ? x.Right?.YamlSettings is { } leftRight ? left.Concat(leftRight) : left
                : x.Right?.YamlSettings,
            x => x.Left?.YamlSettings is { } left
                ? x.Right?.YamlSettings is { } leftRight ? left.Concat(leftRight) : left
                : x.Right?.YamlSettings,
            _ => null,
            _ => null,
            _ => null,
            x => x?.YamlSettings,
            x => [(Builtin.YamlEnabled, x)],
            x => [(Builtin.YamlDisabled, x)],
            x => [(Builtin.YamlCompare, x.Yaml)],
            _ => null
        );

    /// <inheritdoc cref="IArchipelago{T}.op_Implicit(string)"/>
    [Pure]
    public static implicit operator Logic(string name) => OfItem(new(name.AsMemory()));

    /// <inheritdoc cref="IArchipelago{T}.op_Implicit(ReadOnlyMemory{char})"/>
    [Pure]
    public static implicit operator Logic(ReadOnlyMemory<char> name) => OfItem(new(name));

    /// <summary>Swaps the tuple around for <see cref="OfItemCount(Domains.Item, int)"/>.</summary>
    /// <param name="count">The tuple to swap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((int Count, string Item) count) => OfItemCount(count.Item, count.Count);

    /// <summary>Uses <see cref="OfItemCount(Domains.Item, int)"/>.</summary>
    /// <param name="count">The tuple to wrap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((string Item, int Count) count) => OfItemCount(count.Item, count.Count);

    /// <summary>Makes a requirement that either of the instances should be fulfilled.</summary>
    /// <param name="l">The left-hand side.</param>
    /// <param name="r">The right-hand side.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [return: NotNullIfNotNull(nameof(l)), NotNullIfNotNull(nameof(r))]
    [Pure]
    public static Logic? operator |(Logic? l, Logic? r) =>
        l is null ? l.Check(r) : // Identity Law
        r is null ? r.Check(l) :
        l.UnorderedEquals(r) ? l.Check(r) : // Idempotent Law
        //    Input  -> Commutative Law -> Idempotent Law
        // A + B + A ->    A + A + B    ->     A + B
        l is { IsOr: true, Or: var (oll, olr) } && (oll.UnorderedEquals(r) || olr.UnorderedEquals(r)) ? l.Check(r) :
        //    Input  -> Idempotent Law
        // A + A + B ->     A + B
        r is { IsOr: true, Or: var (orl, orr) } && (orl.UnorderedEquals(r) || orr.UnorderedEquals(r)) ? r.Check(l) :
        //    Input    ->  Commutative Law  -> Absorption Law
        // (A * B) + A ->    A + (A * B)    ->       A
        l is { IsAnd: true, And: var (all, alr) } && (all.UnorderedEquals(r) || alr.UnorderedEquals(r)) ? r.Check(l) :
        //    Input    -> Absorption Law
        // A + (A * B) ->       A
        r is { IsAnd: true, And: var (arl, arr) } && (arl.UnorderedEquals(r) || arr.UnorderedEquals(r)) ? l.Check(r) :
        // This code was never in the bible.
        l is { IsOr: true, Or: var (olll, olrl) } && (olrl | r) is { IsOptimized: true } ll ? OfOr(ll, olll) :
        l is { IsOr: true, Or: var (ollr, olrr) } && (ollr | r) is { IsOptimized: true } rl ? OfOr(rl, olrr) :
        r is { IsOr: true, Or: var (orll, orrl) } && (l | orrl) is { IsOptimized: true } lr ? OfOr(orll, lr) :
        r is { IsOr: true, Or: var (orlr, orrr) } && (l | orlr) is { IsOptimized: true } rr ? OfOr(orrr, rr) :
        // We cannot optimize this.
        OfOr(l, r);

    /// <summary>Makes a requirement that both of the instances should be fulfilled.</summary>
    /// <param name="l">The left-hand side.</param>
    /// <param name="r">The right-hand side.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [return: NotNullIfNotNull(nameof(l)), NotNullIfNotNull(nameof(r))]
    [Pure]
    public static Logic? operator &(Logic? l, Logic? r) =>
        l is null ? r.Check(l) : // Annulment Law
        r is null ? l.Check(r) :
        l.UnorderedEquals(r) ? l.Check(r) : // Idempotent Law
        //    Input    ->  Commutative Law -> Absorption Law
        // (A + B) * A ->    A * (A + B)   ->       A
        l is { IsOr: true, Or: var (oll, olr) } &&
        (oll.UnorderedEquals(r) || olr.UnorderedEquals(r)) ? r.Check(l) :
        //    Input    ->  Absorption Law
        // A * (A + B) ->        A
        r is { IsOr: true, Or: var (orl, orr) } &&
        (orl.UnorderedEquals(r) || orr.UnorderedEquals(r)) ? l.Check(r) :
        //   Input   -> Commutative Law -> Idempotent Law
        // A * B * A ->    A * A * B    ->     A * B
        l is { IsAnd: true, And: var (all, alr) } && (all.UnorderedEquals(r) || alr.UnorderedEquals(r)) ? l.Check(r) :
        //   Input   -> Idempotent Law
        // A * A * B ->     A * B
        r is { IsAnd: true, And: var (arl, arr) } && (arl.UnorderedEquals(r) || arr.UnorderedEquals(r)) ? r.Check(l) :
        // This code was never in the bible.
        l is { IsAnd: true, And: var (alll, alrl) } && (alll & r) is { IsOptimized: true } ll ? OfAnd(ll, alrl) :
        l is { IsAnd: true, And: var (allr, alrr) } && (allr & r) is { IsOptimized: true } rl ? OfAnd(rl, alrr) :
        r is { IsAnd: true, And: var (arll, arrl) } && (l & arll) is { IsOptimized: true } lr ? OfAnd(arrl, lr) :
        r is { IsAnd: true, And: var (arlr, arrr) } && (l & arlr) is { IsOptimized: true } rr ? OfAnd(arrr, rr) :
        // We cannot optimize this.
        OfAnd(l, r);

    /// <inheritdoc cref="OfCategoryPercent(Domains.Category, Explicit{int})"/>
    [Pure]
    public static Logic? OfCategoryPercent(Category category, int percent) =>
        percent is 0 ? null : OfCategoryPercent(category, new Explicit<int>(percent));

    /// <inheritdoc cref="OfCategoryPercent(Domains.Category, Explicit{int})"/>
    /// <param name="category">The category.</param>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    [Pure]
    public static Logic? OfCategoryPercent(Category category, double index, double scaling = 1) =>
        index <= 0 ? null : OfCategoryPercent(category, (int)(index * scaling));

    /// <inheritdoc cref="OfItemPercent(Domains.Item, Explicit{int})"/>
    [Pure]
    public static Logic? OfItemPercent(Item item, int percent) =>
        percent is 0 ? null : OfItemPercent(item, new Explicit<int>(percent));

    /// <inheritdoc cref="OfItemPercent(Domains.Item, Explicit{int})"/>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    [Pure]
    public static Logic? OfItemPercent(Item item, double index, double scaling = 1) =>
        index <= 0 ? null : OfItemPercent(item, (int)(index * scaling));

    /// <inheritdoc cref="OfItemValue(Chars, Explicit{int})"/>
    [Pure]
    public static Logic? OfItemValue(Chars phantomItem, int count) =>
        count <= 0 ? null : OfItemValue(phantomItem, new Explicit<int>(count));

    /// <inheritdoc />
    public void CopyTo([NotNullIfNotNull(nameof(value))] ref JsonNode? value, IReadOnlyCollection<Region>? regions) =>
        (value ??= new JsonObject())["requires"] = (ExpandLocations(regions) ?? this).ToString();
#pragma warning disable MA0016
    /// <summary>Throws if an unreferenced element is found.</summary>
    /// <param name="categories">The referenced categories.</param>
    /// <param name="items">The referenced items.</param>
    /// <param name="locations">The referenced locations.</param>
    /// <param name="strict">Whether to perform the check.</param>
    /// <exception cref="KeyNotFoundException">
    /// This logic instance contains an unreferenced category or item.
    /// </exception>
    public void ThrowIfUnreferenced(
        Dictionary<string, Category> categories,
        Dictionary<string, Item> items,
        Dictionary<string, Location> locations,
        bool strict = true
    )
    {
        switch (strict)
        {
            case true when FindUnreferencedCategory(categories) is { Name: var name }:
                throw new KeyNotFoundException($"\"{name}\" is referenced before it is defined in {nameof(Category)}!");
            case true when FindUnreferencedItem(items) is { Name: var name }:
                throw new KeyNotFoundException($"\"{name}\" is referenced before it is defined in {nameof(Item)}!");
            case true when FindUnreferencedLocation(locations) is { Name: var name }:
                throw new KeyNotFoundException($"\"{name}\" is referenced before it is defined in {nameof(Location)}!");
        }
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => ToString(false);

    /// <summary>Attempts to find a category whose key does not already exist in the provided collection.</summary>
    /// <param name="collection">The collection that has defined keys.</param>
    /// <returns>The category whose key does not exist in the collection, if one exists.</returns>
    [Pure]
    public Category? FindUnreferencedCategory(Dictionary<string, Category> collection) =>
        Map(
            x => FindAnyUnreferencedCategory(x.Categories, collection),
            x => FindAnyUnreferencedCategory(x.Item.Categories, collection),
            x => FindAnyUnreferencedCategory(x.Item.Categories, collection),
            x => IsUnreferenced(x, collection) ? (Category?)x : null,
            x => IsUnreferenced(x.Category, collection) ? (Category?)x.Category : null,
            x => IsUnreferenced(x.Category, collection) ? (Category?)x.Category : null,
            x => x.Left?.FindUnreferencedCategory(collection) ?? x.Right?.FindUnreferencedCategory(collection),
            x => x.Left?.FindUnreferencedCategory(collection) ?? x.Right?.FindUnreferencedCategory(collection),
            x => FindAnyUnreferencedCategory(x.Categories, collection),
            _ => null,
            _ => null,
            x => x?.FindUnreferencedCategory(collection),
            _ => null,
            _ => null,
            _ => null,
            _ => null
        );

    /// <summary>Attempts to find an item whose key does not already exist in the provided collection.</summary>
    /// <param name="collection">The collection that has defined keys.</param>
    /// <returns>The item whose key does not exist in the collection, if one exists.</returns>
    [Pure]
    public Item? FindUnreferencedItem(Dictionary<string, Item> collection) =>
        Map(
            x => IsUnreferenced(x, collection) ? (Item?)x : null,
            x => IsUnreferenced(x.Item, collection) ? (Item?)x.Item : null,
            x => IsUnreferenced(x.Item, collection) ? (Item?)x.Item : null,
            _ => null,
            _ => null,
            _ => null,
            x => x.Left?.FindUnreferencedItem(collection) ?? x.Right?.FindUnreferencedItem(collection),
            x => x.Left?.FindUnreferencedItem(collection) ?? x.Right?.FindUnreferencedItem(collection),
            _ => null,
            _ => null,
            _ => null,
            x => x?.FindUnreferencedItem(collection),
            _ => null,
            _ => null,
            _ => null,
            _ => null
        );

    /// <summary>Attempts to find an item whose key does not already exist in the provided collection.</summary>
    /// <param name="collection">The collection that has defined keys.</param>
    /// <returns>The item whose key does not exist in the collection, if one exists.</returns>
    [Pure]
    public Location? FindUnreferencedLocation(Dictionary<string, Location> collection) =>
        IsLocation && IsUnreferenced(Location, collection) ? (Location?)Location : null;

    /// <summary>Inlines all use of <see cref="Builtin.CanReachLocation"/>.</summary>
    /// <param name="regions">The regions.</param>
    /// <returns>The inlined logic.</returns>
    [Pure]
    public Logic? ExpandLocations(IReadOnlyCollection<Region>? regions) =>
        regions is null ? null :
            _or is ({ } left, { } right) ?
                left.ExpandLocations(regions) is { } l && (right.ExpandLocations(regions) ?? right) is var ir ?
                    IsOr ? l | ir : l & ir :
                    right.ExpandLocations(regions) is { } r ? IsOr ? left | r : left & r : null :
                IsLocation ? Location.Logic & Location.Region.ExpandedLogic(regions) : null;

    /// <summary>Gets the value determining whether the item is unreferenced.</summary>
    /// <typeparam name="TArchipelago">The type of value to check.</typeparam>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <param name="ap">The value to check.</param>
    /// <param name="dictionary">The dictionary to compare against.</param>
    /// <returns>
    /// Whether the parameter <paramref name="ap"/> is not referenced in the parameter <paramref name="dictionary"/>.
    /// </returns>
    [Pure]
    static bool IsUnreferenced<TArchipelago, TValue>(TArchipelago ap, Dictionary<string, TValue> dictionary)
        where TArchipelago : IArchipelago<TArchipelago> =>
        Unsafe.IsNullRef(
            CollectionsMarshal.GetValueRefOrNullRef(dictionary.GetAlternateLookup<ReadOnlySpan<char>>(), ap.Name.Span)
        );

    [Pure]
    static string Percent<T>((T Item, Explicit<int> Percent) x)
        where T : IArchipelago<T> =>
        x.Percent.Value switch
        {
            100 => "ALL",
            50 => "HALF",
            var y => $"{y}%",
        };

    [Pure]
    static string Symbol(ExpressionType type) =>
        type switch
        {
            ExpressionType.LessThan => "<",
            ExpressionType.GreaterThan => ">",
            ExpressionType.LessThanOrEqual => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.NotEqual or ExpressionType.Not => "!=",
            _ => "=",
        };

    [Pure]
    static Category? FindAnyUnreferencedCategory(ImmutableArray<Category> x, Dictionary<string, Category> collection) =>
        x.Select(x => IsUnreferenced(x, collection) ? (Category?)x : null).FirstOrDefault(x => x is not null);

    /// <inheritdoc cref="object.ToString"/>
    /// <param name="state">The state.</param>
    [Pure]
    string ToString(bool? state) =>
        Map(
            x => $"|{x.Name}|",
            x => $"|{x.Item.Name}:{x.Count}|",
            x => $"|{x.Item.Name}:{Percent(x)}|",
            x => $"|@{x.Name}|",
            x => $"|@{x.Category.Name}:{x.Count}|",
            x => $"|@{x.Category.Name}:{Percent(x)}|",
            state is false
                ? x => $"({x.Left?.ToString(true)} OR {x.Right?.ToString(true)})"
                : x => $"{x.Left?.ToString(true)} OR {x.Right?.ToString(true)}",
            state is true
                ? x => $"({x.Left?.ToString(false)} AND {x.Right?.ToString(false)})"
                : x => $"{x.Left?.ToString(false)} AND {x.Right?.ToString(false)}",
            x => $"{{canReachLocation({x.Name})}}",
            x => $"{{{nameof(Builtin.ItemValue)}({x.PhantomItem}:{x.Count})}}",
            x => $"{{{nameof(Builtin.OptOne)}({x})}}",
            x => $"{{{nameof(Builtin.OptAll)}({x})}}",
            x => $"{{{nameof(Builtin.YamlEnabled)}({x})}}",
            x => $"{{{nameof(Builtin.YamlDisabled)}({x})}}",
            x => $"{{{nameof(Builtin.YamlCompare)}({x.Yaml} {Symbol(x.Comparator)} {x.Count})}}",
            x => $"{{{x.Name}({x.Args})}}"
        );
}
