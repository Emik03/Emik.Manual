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
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [return: NotNullIfNotNull(nameof(left)), NotNullIfNotNull(nameof(right))]
    [Pure]
    public static Logic? operator |(Logic? left, Logic? right) =>
        left is null ? right :
        right is null ? left : OfOr(left, right);

    /// <summary>Makes a requirement that both of the instances should be fulfilled.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [return: NotNullIfNotNull(nameof(left)), NotNullIfNotNull(nameof(right))]
    [Pure]
    public static Logic? operator &(Logic? left, Logic? right) =>
        left is null ? right :
        right is null ? left : OfAnd(left, right);

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
    void IAddTo.CopyTo(ref JsonNode? value) => (value ??= new JsonObject())["requires"] = ToString();
#pragma warning disable MA0016
    /// <summary>Throws if an unreferenced element is found.</summary>
    /// <param name="categories">The referenced categories.</param>
    /// <param name="items">The referenced items.</param>
    /// <param name="strict">Whether to perform the check.</param>
    /// <exception cref="KeyNotFoundException">
    /// This logic instance contains an unreferenced category or item.
    /// </exception>
    public void ThrowIfUnreferenced(
        Dictionary<string, Category> categories,
        Dictionary<string, Item> items,
        bool strict = true
    )
    {
        switch (strict)
        {
            case true when FindUnreferencedCategory(categories) is { Name: var name }:
                throw new KeyNotFoundException($"\"{name}\" is referenced before it is defined in {nameof(Category)}!");
            case true when FindUnreferencedItem(items) is { Name: var name }:
                throw new KeyNotFoundException($"\"{name}\" is referenced before it is defined in {nameof(Item)}!");
        }
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() =>
        Map(
            x => $"|{x.Name}|",
            x => $"|{x.Item.Name}:{x.Count}|",
            x => $"|{x.Item.Name}:{Percent(x)}|",
            x => $"|@{x.Name}|",
            x => $"|@{x.Category.Name}:{x.Count}|",
            x => $"|@{x.Category.Name}:{Percent(x)}|",
            x => $"({x.Left} OR {x.Right})",
            x => $"({x.Left} AND {x.Right})",
            x => $"{{{nameof(Builtin.ItemValue)}({x.PhantomItem}:{x.Count})}}",
            x => $"{{{nameof(Builtin.OptOne)}({x})}}",
            x => $"{{{nameof(Builtin.OptAll)}({x})}}",
            x => $"{{{nameof(Builtin.YamlEnabled)}({x})}}",
            x => $"{{{nameof(Builtin.YamlDisabled)}({x})}}",
            x => $"{{{nameof(Builtin.YamlCompare)}({x.Yaml} {Symbol(x.Comparator)} {x.Count})}}",
            x => $"{{{x.Name}({x.Args})}}"
        );

    /// <summary>Attempts to find a category whose key does not already exist in the provided collection.</summary>
    /// <param name="collection">The collection that has defined keys.</param>
    /// <returns>The category whose key does not exist in the collection, if one exists.</returns>
    [Pure]
    public Category? FindUnreferencedCategory(Dictionary<string, Category> collection) =>
#pragma warning restore MA0016
        Map(
            _ => null,
            _ => null,
            _ => null,
            x => IsUnreferenced(x, collection) ? (Category?)x : null,
            x => IsUnreferenced(x.Category, collection) ? (Category?)x.Category : null,
            x => IsUnreferenced(x.Category, collection) ? (Category?)x.Category : null,
            x => x.Left?.FindUnreferencedCategory(collection) ?? x.Right?.FindUnreferencedCategory(collection),
            x => x.Left?.FindUnreferencedCategory(collection) ?? x.Right?.FindUnreferencedCategory(collection),
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
#pragma warning disable MA0016
    public Item? FindUnreferencedItem(Dictionary<string, Item> collection) =>
#pragma warning restore MA0016
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
            x => x?.FindUnreferencedItem(collection),
            _ => null,
            _ => null,
            _ => null,
            _ => null
        );

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
}
