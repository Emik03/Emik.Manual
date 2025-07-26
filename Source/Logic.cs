// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;
#pragma warning disable CS9113, MA0016
/// <summary>
/// Represents the set of requirements for <see cref="Location"/> or <see cref="Region"/> to be considered reachable.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public sealed partial class Logic : IAddTo,
    IArchipelago<Logic>,
    IComparable,
    IComparable<object>,
    IComparable<Logic>,
    IComparisonOperators<Logic, Logic, bool>,
    IEquatable<object>,
    IEquatable<Logic>
{
    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    public Logic(string? item) => (Name, Type) = (item, Kind.Item);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    public Logic(Chars item) => (Name, Type) = (item, Kind.Item);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    public Logic(ReadOnlyMemory<char> item) => (Name, Type) = (item, Kind.Item);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    public Logic(in Item item) => (Name, Type) = (item.Name, Kind.Item);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    /// <param name="count">The count.</param>
    public Logic(string item, int count) => (Name, Count, Type) = (item, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    /// <param name="count">The count.</param>
    public Logic(Chars item, int count) => (Name, Count, Type) = (item, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    /// <param name="count">The count.</param>
    public Logic(ReadOnlyMemory<char> item, int count) => (Name, Count, Type) = (item, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="item">The item.</param>
    /// <param name="count">The count.</param>
    public Logic(in Item item, int count) => (Name, Count, Type) = (item.Name, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="count">The count.</param>
    /// <param name="item">The item.</param>
    public Logic(int count, string item) => (Name, Count, Type) = (item, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="count">The count.</param>
    /// <param name="item">The item.</param>
    public Logic(int count, Chars item) => (Name, Count, Type) = (item, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="count">The count.</param>
    /// <param name="item">The item.</param>
    public Logic(int count, ReadOnlyMemory<char> item) => (Name, Count, Type) = (item, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="count">The count.</param>
    /// <param name="item">The item.</param>
    public Logic(int count, in Item item) => (Name, Count, Type) = (item.Name, count, Kind.ItemCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="category">The category.</param>
    public Logic(in Category category) => (Name, Type) = (category.Name, Kind.Category);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="category">The category.</param>
    /// <param name="count">The count.</param>
    public Logic(in Category category, int count) =>
        (Name, Count, Type) = (category.Name, count, Kind.CategoryCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="count">The count.</param>
    /// <param name="category">The category.</param>
    public Logic(int count, in Category category) =>
        (Name, Count, Type) = (category.Name, count, Kind.CategoryCount);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="region">The region.</param>
    public Logic(in Region region) => (Name, Type) = (region.Name, Kind.Region);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="passage">The passage.</param>
    public Logic(in Passage passage)
    {
        if (passage.SelfLogic.IsRedundant())
            (Name, Type) = (passage.Region.Name, Kind.Region);
        else
            (Left, Right, Type) = (passage.Region, passage.SelfLogic, Kind.And);
    }

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="location">The location.</param>
    public Logic(in Location location) => (Name, Type) = (location.Name, Kind.Location);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="chars">The chars.</param>
    /// <param name="count">The count.</param>
    /// <param name="kind">The kind.</param>
    internal Logic(Chars chars, int count, Kind kind) => (Name, Count, Type) = (chars, count, kind);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="opt">The option.</param>
    internal Logic(Logic? opt) =>
        (Left, Type) = (opt, opt?.Type is Kind.Item or Kind.ItemCount or Kind.Category or Kind.CategoryCount
            ? Kind.OptOne
            : Kind.OptAll);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="chars">The chars.</param>
    /// <param name="kind">The kind.</param>
    Logic(Chars chars, Kind kind) => (Name, Type) = (chars, kind);

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="name">The name.</param>
    /// <param name="args">The args.</param>
    Logic(Chars name, ReadOnlySpan<Chars> args)
    {
        static void AppendArgs(StringBuilder builder, ReadOnlySpan<Chars> args)
        {
            var e = args.GetEnumerator();

            if (!e.MoveNext())
                return;

            builder.Append(e.Current);

            while (e.MoveNext())
                builder.Append(',').Append(e.Current);
        }

        Type = Kind.Custom;
        StringBuilder builder = new();
        AppendArgs(builder.Append(name.Span).Append('('), args);
        Name = builder.Append(')').ToString();
    }

    /// <summary>Initializes a new instance of the <see cref="Logic"/> class.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <param name="kind">The kind.</param>
    Logic(Logic? left, Logic? right, Kind kind) => (Left, Right, Type) = (left, right, kind);

    /// <summary>Determines whether this node is optimized.</summary>
    [Pure]
    public bool IsOptimized { get; internal set; }

    /// <inheritdoc />
    [Pure]
    public int Count { get; }

    /// <summary>Gets the kind of node this is.</summary>
    [Pure]
    public Kind Type { get; }

    /// <inheritdoc />
    [Pure]
    public Chars Name { get; }

    /// <summary>Gets the left branch, or the logic within <see cref="Builtin.OptAll"/>.</summary>
    [Pure]
    public Logic? Left { get; }

    /// <summary>Gets the right branch.</summary>
    [Pure]
    public Logic? Right { get; }

    /// <summary>Gets the builtin kind.</summary>
    public Builtin? BuiltinKind =>
        Type switch
        {
            Kind.ItemValue => Builtin.ItemValue,
            Kind.OptOne => Builtin.OptOne,
            Kind.OptAll => Builtin.OptAll,
            Kind.YamlEnabled => Builtin.YamlEnabled,
            Kind.YamlDisabled => Builtin.YamlDisabled,
            Kind.YamlEqual or
                Kind.YamlNotEqual or
                Kind.YamlGreaterThan or
                Kind.YamlGreaterThanOrEqual or
                Kind.YamlLessThan or
                Kind.YamlLessThanOrEqual => Builtin.YamlCompare,
            _ => null,
        };

    /// <summary>
    /// Gets the yaml settings that this <see cref="Logic"/> references. This value is <see langword="null"/> if there
    /// are no yaml settings defined within this logic. Therefore, this collection can never be empty.
    /// </summary>
    [Pure]
    public IEnumerable<(Builtin Context, Yaml Yaml)>? YamlSettings =>
        Type switch
        {
            Kind.And or Kind.Or => Left?.YamlSettings is { } left
                ? Right?.YamlSettings is { } leftRight ? left.Concat(leftRight) : left
                : Right?.YamlSettings,
            Kind.OptOne or Kind.OptAll => Left?.YamlSettings,
            Kind.YamlEnabled or
                Kind.YamlDisabled or
                Kind.YamlEqual or
                Kind.YamlNotEqual or
                Kind.YamlGreaterThan or
                Kind.YamlGreaterThanOrEqual or
                Kind.YamlLessThan or
                Kind.YamlLessThanOrEqual => [(Builtin.YamlCompare, new(Name))],
            _ => null,
        };

    /// <summary>Gets the percent.</summary>
    /// <returns>The percent.</returns>
    [Pure]
    string Percentage => Count switch { 50 => "HALF", 100 => "ALL", _ => $"{Count}%" };

    /// <inheritdoc cref="IArchipelago{T}.op_Implicit(string)"/>
    [Pure]
    public static implicit operator Logic(string? name) => new(name);

    /// <summary>Implicitly invokes <see cref="Manual.Logic(Chars)"/>.</summary>
    /// <param name="item">The item.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic(Chars item) => new(item);

    /// <inheritdoc cref="IArchipelago{T}.op_Implicit(ReadOnlyMemory{char})"/>
    [Pure]
    public static implicit operator Logic(ReadOnlyMemory<char> name) => new(name);

    /// <summary>Implicitly invokes <see cref="Manual.Logic(in Item)"/>.</summary>
    /// <param name="item">The item.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic(in Item item) => new(item);

    /// <summary>Uses <see cref="Manual.Logic(string, int)"/>.</summary>
    /// <param name="count">The tuple to wrap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((string Item, int Count) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(Chars, int)"/>.</summary>
    /// <param name="count">The tuple to wrap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((Chars Item, int Count) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(ReadOnlyMemory{char}, int)"/>.</summary>
    /// <param name="count">The tuple to wrap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((ReadOnlyMemory<char> Item, int Count) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(in Item, int)"/>.</summary>
    /// <param name="count">The tuple to wrap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((Item Item, int Count) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(int, string)"/>.</summary>
    /// <param name="count">The tuple to swap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((int Count, string Item) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(int, Chars)"/>.</summary>
    /// <param name="count">The tuple to swap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((int Count, Chars Item) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(int, ReadOnlyMemory{char})"/>.</summary>
    /// <param name="count">The tuple to swap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((int Count, ReadOnlyMemory<char> Item) count) => new(count.Item, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(int, in Item)"/>.</summary>
    /// <param name="count">The tuple to swap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((int Count, Item Item) count) => new(count.Item, count.Count);

    /// <summary>Implicitly invokes <see cref="Manual.Logic(in Category)"/>.</summary>
    /// <param name="category">The category.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic(in Category category) => new(category);

    /// <summary>Uses <see cref="Manual.Logic(in Category, int)"/>.</summary>
    /// <param name="count">The tuple to wrap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((Category Category, int Count) count) => new(count.Category, count.Count);

    /// <summary>Uses <see cref="Manual.Logic(int, in Category)"/>.</summary>
    /// <param name="count">The tuple to swap.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic((int Count, Category Category) count) => new(count.Category, count.Count);

    /// <summary>Implicitly invokes <see cref="Manual.Logic(in Region)"/>.</summary>
    /// <param name="region">The region.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic(in Region region) => new(region);

    /// <summary>Implicitly invokes <see cref="Manual.Logic(in Passage)"/>.</summary>
    /// <param name="passage">The passage.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic(in Passage passage) => new(passage);

    /// <summary>Implicitly invokes <see cref="Manual.Logic(in Location)"/>.</summary>
    /// <param name="location">The location.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static implicit operator Logic(in Location location) => new(location);

    /// <summary>Determines whether logic contains structurally the same data.</summary>
    /// <param name="left">The logic to compare from.</param>
    /// <param name="right">The logic to compare to.</param>
    /// <returns>Whether both instances are equal.</returns>
    public static bool operator ==(Logic? left, Logic? right) =>
        ReferenceEquals(left, right) ||
        left is not null &&
        right is not null &&
        left.Type == right.Type &&
        left.Count == right.Count &&
        left.Name == right.Name &&
        (left.Left == right.Left && left.Right == right.Right ||
            left.Right == right.Left && left.Left == right.Right);

    /// <summary>Determines whether the left-hand side is unequal to the right.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value determining whether the parameter <paramref name="left"/>
    /// is unequal to the parameter <paramref name="right"/>.
    /// </returns>
    public static bool operator !=(Logic? left, Logic? right) => !(left == right);

    /// <summary>Determines whether logic contains structurally the same data.</summary>
    /// <param name="left">The logic to compare from.</param>
    /// <param name="right">The logic to compare to.</param>
    /// <returns>Whether both instances are equal.</returns>
    public static bool operator >(Logic? left, Logic? right) =>
        right is not null &&
        (left is null ||
            !ReferenceEquals(left, right) &&
            left.Type > right.Type &&
            left.Count > right.Count &&
            left.Name.Span.CompareTo(right.Name.Span, StringComparison.Ordinal) > 0 &&
            left.Left > right.Left &&
            left.Right > right.Right);

    /// <summary>Determines whether the left-hand side is less than the right.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value determining whether the parameter <paramref name="left"/>
    /// is less than the parameter <paramref name="right"/>.
    /// </returns>
    public static bool operator <(Logic? left, Logic? right) => right > left;

    /// <summary>
    /// Determines whether the left-hand side is greater than or equal to the right.
    /// </summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value determining whether the parameter <paramref name="left"/>
    /// is greater than or equal to the parameter <paramref name="right"/>.
    /// </returns>
    public static bool operator >=(Logic? left, Logic? right) =>
        left is null ||
        ReferenceEquals(left, right) ||
        right is not null &&
        left.Type >= right.Type &&
        left.Count >= right.Count &&
        left.Name.Span.CompareTo(right.Name.Span, StringComparison.Ordinal) >= 0 &&
        left.Left >= right.Left &&
        left.Right >= right.Right;

    /// <summary>
    /// Determines whether the left-hand side is less than or equal to the right.
    /// </summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>
    /// The value determining whether the parameter <paramref name="left"/>
    /// is less than or equal to the parameter <paramref name="right"/>.
    /// </returns>
    public static bool operator <=(Logic? left, Logic? right) => right >= left;

    /// <summary>Makes a requirement that either of the instances should be fulfilled.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [return: NotNullIfNotNull(nameof(left)), NotNullIfNotNull(nameof(right))]
    [Pure]
    public static Logic? operator |(Logic? left, Logic? right) =>
        left is null ? right.Check(left) : // Annulment Law; Cannot be Identity Law
        right is null ? left.Check(right) :
        left == right ? left.Check(right) : // Idempotent Law
        //    Input  -> Commutative Law -> Idempotent Law
        // A + B + A ->    A + A + B    ->     A + B
        left.Type is Kind.Or && (left.Left == right || left.Right == right) ? left.Check(right) :
        //    Input  -> Idempotent Law
        // A + A + B ->     A + B
        right.Type is Kind.Or && (right.Left == right || right.Right == right) ? right.Check(left) :
        //    Input    ->  Commutative Law  -> Absorption Law
        // (A * B) + A ->    A + (A * B)    ->       A
        left.Type is Kind.And && (left.Left == right || left.Right == right) ? right.Check(left) :
        //    Input    -> Absorption Law
        // A + (A * B) ->       A
        right.Type is Kind.And && (right.Left == right || right.Right == right) ? left.Check(right) :
        // This code was never in the bible.
        left.Type is Kind.Or && (left.Right | right) is { IsOptimized: true } ll ? new(ll, left.Left, Kind.Or) :
        left.Type is Kind.Or && (left.Left | right) is { IsOptimized: true } rl ? new(rl, left.Right, Kind.Or) :
        right.Type is Kind.Or && (left | right.Right) is { IsOptimized: true } lr ? new(right.Left, lr, Kind.Or) :
        right.Type is Kind.Or && (left | right.Left) is { IsOptimized: true } rr ? new(right.Right, rr, Kind.Or) :
        // We cannot optimize this.
        new(left, right, Kind.Or);

    /// <summary>Makes a requirement that both of the instances should be fulfilled.</summary>
    /// <param name="left">The left-hand side.</param>
    /// <param name="right">The right-hand side.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [return: NotNullIfNotNull(nameof(left)), NotNullIfNotNull(nameof(right))]
    [Pure]
    public static Logic? operator &(Logic? left, Logic? right) =>
        left is null ? right.Check(left) : // Annulment Law
        right is null ? left.Check(right) :
        left == right ? left.Check(right) : // Idempotent Law
        //    Input    ->  Commutative Law -> Absorption Law
        // (A + B) * A ->    A * (A + B)   ->       A
        left.Type is Kind.Or && (left.Left == right || left.Right == right) ? right.Check(left) :
        //    Input    ->  Absorption Law
        // A * (A + B) ->        A
        right.Type is Kind.Or && (right.Left == right || right.Right == right) ? left.Check(right) :
        //   Input   -> Commutative Law -> Idempotent Law
        // A * B * A ->    A * A * B    ->     A * B
        left.Type is Kind.And && (left.Left == right || left.Right == right) ? left.Check(right) :
        //   Input   -> Idempotent Law
        // A * A * B ->     A * B
        right.Type is Kind.And && (right.Left == right || right.Right == right) ? right.Check(left) :
        // This code was never in the bible.
        left.Type is Kind.And && (left.Left & right) is { IsOptimized: true } ll ? new(ll, left.Right, Kind.And) :
        left.Type is Kind.And && (left.Left & right) is { IsOptimized: true } rl ? new(rl, left.Right, Kind.And) :
        right.Type is Kind.And && (left & right.Left) is { IsOptimized: true } lr ? new(right.Right, lr, Kind.And) :
        right.Type is Kind.And && (left & right.Left) is { IsOptimized: true } rr ? new(right.Right, rr, Kind.And) :
        // We cannot optimize this.
        new(left, right, Kind.And);

    /// <summary>Creates the <see cref="Logic"/> object of <see cref="Builtin.YamlDisabled"/>.</summary>
    /// <param name="yaml">The yaml.</param>
    /// <returns>The new <see cref="Logic"/> object containing the parameter <paramref name="yaml"/>.</returns>
    /// <seealso cref="Builtin.YamlDisabled"/>
    [Pure]
    public static Logic Disabled(Yaml yaml) => new(yaml.Name, Kind.YamlDisabled);

    /// <summary>Creates the <see cref="Logic"/> object of <see cref="Builtin.YamlEnabled"/>.</summary>
    /// <param name="yaml">The yaml.</param>
    /// <returns>The new <see cref="Logic"/> object containing the parameter <paramref name="yaml"/>.</returns>
    /// <seealso cref="Builtin.YamlEnabled"/>
    [Pure]
    public static Logic Enabled(Yaml yaml) => new(yaml.Name, Kind.YamlEnabled);

    /// <summary>Creates the <see cref="Logic"/> requiring a percentage of a given <see cref="Item"/>.</summary>
    /// <param name="category">The category.</param>
    /// <param name="percent">The percent.</param>
    /// <returns>
    /// The <see cref="Logic"/> object requiring a specific amount of the parameter <paramref name="category"/>.
    /// </returns>
    [Pure]
    public static Logic? Percent(in Category category, int percent) =>
        percent > 0 ? new(category.Name, percent, Kind.CategoryPercent) : null;

    /// <summary>Creates the <see cref="Logic"/> requiring a percentage of a given <see cref="Category"/>.</summary>
    /// <param name="category">The category.</param>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    /// <returns>
    /// The <see cref="Logic"/> object requiring a specific amount of the parameter <paramref name="category"/>.
    /// </returns>
    [Pure]
    public static Logic? Percent(in Category category, double index, double scaling = 1) =>
        Percent(category, (int)(index * scaling));

    /// <summary>Creates the <see cref="Logic"/> requiring a percentage of a given <see cref="Item"/>.</summary>
    /// <param name="item">The item.</param>
    /// <param name="percent">The percent.</param>
    /// <returns>
    /// The <see cref="Logic"/> object requiring a specific amount of the parameter <paramref name="item"/>.
    /// </returns>
    [Pure]
    public static Logic? Percent(in Item item, int percent) =>
        percent > 0 ? new(item.Name, percent, Kind.ItemPercent) : null;

    /// <summary>Creates the <see cref="Logic"/> requiring a percentage of a given <see cref="Item"/>.</summary>
    /// <param name="item">The item.</param>
    /// <param name="index">The index.</param>
    /// <param name="scaling">The scaling per index.</param>
    /// <returns>
    /// The <see cref="Logic"/> object requiring a specific amount of the parameter <paramref name="item"/>.
    /// </returns>
    [Pure]
    public static Logic? Percent(in Item item, double index, double scaling = 1) =>
        Percent(item, (int)(index * scaling));

    /// <summary>Creates the <see cref="Logic"/> object of <see cref="Builtin.ItemValue"/>.</summary>
    /// <param name="phantomItem">The phantom item.</param>
    /// <param name="count">The amount needed.</param>
    /// <returns>
    /// The new <see cref="Logic"/> object containing the parameter
    /// <paramref name="phantomItem"/> and <paramref name="count"/>.
    /// </returns>
    /// <seealso cref="Builtin.ItemValue"/>
    [Pure]
    public static Logic? ItemValue(Chars phantomItem, int count) =>
        count > 0 ? new(phantomItem, count, Kind.ItemValue) : null;

    /// <summary>Creates the <see cref="Logic"/> object of <see cref="Builtin.Custom"/>.</summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="args">The args of the function.</param>
    /// <returns>The new <see cref="Logic"/> object containing the parameter <paramref name="name"/>.</returns>
    /// <seealso cref="Builtin.Custom"/>
    [Pure]
    public static Logic Custom(Chars name, params ReadOnlySpan<Chars> args) => new(name, args);

    /// <inheritdoc />
    public void CopyTo(
        [NotNullIfNotNull(nameof(value))] ref JsonNode? value,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    ) =>
        (value ??= new JsonObject())["requires"] = (ExpandLocations(locations, regions) ?? this).ToString();

    /// <summary>Throws if an unreferenced element is found.</summary>
    /// <param name="categories">The referenced categories.</param>
    /// <param name="items">The referenced items.</param>
    /// <param name="locations">The referenced locations.</param>
    /// <param name="regions">The referenced regions.</param>
    /// <param name="strict">Whether to perform the check.</param>
    /// <exception cref="KeyNotFoundException">
    /// This <see cref="Logic"/> instance contains an unreferenced <see cref="Category"/>,
    /// <see cref="Item"/>, <see cref="Location"/>, or <see cref="Region"/>.
    /// </exception>
    public void ThrowIfUnset(
        Dictionary<string, Category> categories,
        Dictionary<string, Item> items,
        Dictionary<string, Location> locations,
        Dictionary<string, Region> regions,
        bool strict = true
    )
    {
        [DoesNotReturn]
        static void Throw<T>(in T t)
            where T : IArchipelago<T> =>
            throw new KeyNotFoundException($"\"{t.Name}\" is referenced before it is defined in {typeof(T).Name}!");

        [Pure]
        static Category? FindFirstUnreferenced(ImmutableArray<Category> array, Dictionary<string, Category> dict)
        {
            if (array.IsDefaultOrEmpty)
                return null;

            foreach (var c in array.AsSpan())
                if (Unsafe.IsNullRef(
                    CollectionsMarshal.GetValueRefOrNullRef(dict.GetAlternateLookup<ReadOnlySpan<char>>(), c.Name.Span)
                ))
                    return c;

            return null;
        }

        switch (Type)
        {
            case var _ when !strict: break;
            case Kind.And or Kind.Or:
                Left?.ThrowIfUnset(categories, items, locations, regions);
                Right?.ThrowIfUnset(categories, items, locations, regions);
                break;
            case Kind.Item or Kind.ItemCount or Kind.ItemPercent:
                if (IsUnset(items, out var item))
                    Throw(item);

                if (FindFirstUnreferenced(item.Categories, categories) is { } itemCategory)
                    Throw(itemCategory);

                break;
            case Kind.Category or Kind.CategoryCount or Kind.CategoryPercent:
                if (IsUnset(categories, out var category))
                    Throw(category);

                break;
            case Kind.Region:
                if (IsUnset(regions, out var region))
                    Throw(region);

                break;
            case Kind.Location:
                if (IsUnset(locations, out var location))
                    Throw(location);

                if (FindFirstUnreferenced(location.Categories, categories) is { } locationCategory)
                    Throw(locationCategory);

                break;
            case Kind.OptOne or Kind.OptAll:
                Left?.ThrowIfUnset(categories, items, locations, regions);
                break;
        }
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    [Pure]
    public override bool Equals(object? obj) => Equals(obj as Logic);

    /// <inheritdoc />
    [Pure]
    public bool Equals(Logic? obj) => this == obj;

    /// <inheritdoc cref="IComparable.CompareTo"/>
    public int CompareTo(object? obj) => CompareTo(obj as Logic);

    /// <inheritdoc />
    public int CompareTo(Logic? other) =>
        other is null ? -1 :
        ReferenceEquals(this, other) ? 0 :
        Type != other.Type ? Type > other.Type ? 1 : -1 :
        Count != other.Count ? Count > other.Count ? 1 : -1 :
        Name.Span.CompareTo(other.Name.Span, StringComparison.Ordinal) is not 0 and var chars ? chars :
        Left is null ? other.Left is null ? 0 : -1 :
        Left.CompareTo(other.Left) is not 0 and var left ? left :
        Right?.CompareTo(other.Right) ?? (other.Right is null ? 0 : -1);

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode()
    {
        var hash = unchecked((int)Type * 74207281);
        hash ^= unchecked(Count * 57885161);
        hash ^= unchecked(Name.Span.GetDjb2HashCode() * 43112609);

        if (Left is not null)
            hash ^= Left.GetHashCode();

        if (Right is not null)
            hash ^= Right.GetHashCode();

        return hash;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => ToString(false);

    /// <summary>Inlines all use of <see cref="Builtin.CanReachLocation"/>.</summary>
    /// <param name="locations">The locations.</param>
    /// <param name="regions">The regions.</param>
    /// <returns>The inlined logic.</returns>
    [Pure]
    public Logic? ExpandLocations(Dictionary<string, Location>? locations, Dictionary<string, Region>? regions) =>
        Type switch
        {
            _ when regions is null => null,
            Kind.And => Left?.ExpandLocations(locations, regions) is { } l &&
                (Right?.ExpandLocations(locations, regions) ?? Right) is var ir ? l & ir :
                Right?.ExpandLocations(locations, regions) is { } r ? Left & r : null,
            Kind.Or => Left?.ExpandLocations(locations, regions) is { } l &&
                (Right?.ExpandLocations(locations, regions) ?? Right) is var ir ? l | ir :
                Right?.ExpandLocations(locations, regions) is { } r ? Left | r : null,
            Kind.OptOne or Kind.OptAll when Left?.ExpandLocations(locations, regions) is { } opt => new(opt),
            Kind.Region when !IsUnset(regions, out var r) => r.ExpandedLogic(regions.Values),
            Kind.Location when !IsUnset(locations, out var l) => l.SelfLogic & l.Region.ExpandedLogic(regions.Values),
            _ => null,
        };

    /// <summary>Gets the value determining whether the item is unreferenced.</summary>
    /// <typeparam name="TValue">The type of value in the dictionary.</typeparam>
    /// <param name="dict">The dictionary to compare against.</param>
    /// <param name="result">The result.</param>
    /// <returns>
    /// Whether the field <see cref="Name"/> is not referenced in the parameter <paramref name="dict"/>.
    /// </returns>
    [Pure]
    bool IsUnset<TValue>(Dictionary<string, TValue>? dict, out TValue result)
    {
        if (dict is null)
        {
            Unsafe.SkipInit(out result);
            return true;
        }

        result = CollectionsMarshal.GetValueRefOrNullRef(dict.GetAlternateLookup<ReadOnlySpan<char>>(), Name.Span);
        return Unsafe.IsNullRef(result);
    }

    /// <inheritdoc cref="object.ToString"/>
    /// <param name="state">The state.</param>
    [Pure]
    string ToString(bool? state) =>
        Type switch
        {
            Kind.And => state is false
                ? $"({Left?.ToString(true)} AND {Right?.ToString(true)})"
                : $"{Left?.ToString(true)} AND {Right?.ToString(true)}",
            Kind.Or => state is true
                ? $"({Left?.ToString(false)} OR {Right?.ToString(false)})"
                : $"{Left?.ToString(false)} OR {Right?.ToString(false)}",
            Kind.Item => $"|{Name}|",
            Kind.ItemCount => $"|{Name}:{Count}|",
            Kind.ItemPercent => $"|{Name}:{Percentage}|",
            Kind.Category => $"|@{Name}|",
            Kind.CategoryCount => $"|@{Name}:{Count}|",
            Kind.CategoryPercent => $"|@{Name}:{Percentage}|",
            Kind.Region => $"{{canReachRegion({Name})}}",
            Kind.Location => $"{{canReachLocation({Name})}}",
            Kind.ItemValue => $"{{{nameof(Builtin.ItemValue)}({Name}:{Count})}}",
            Kind.OptOne => $"{{{nameof(Builtin.OptOne)}({Left})}}",
            Kind.OptAll => $"{{{nameof(Builtin.OptAll)}({Left})}}",
            Kind.YamlEnabled => $"{{{nameof(Builtin.YamlEnabled)}({Name})}}",
            Kind.YamlDisabled => $"{{{nameof(Builtin.YamlDisabled)}({Name})}}",
            Kind.YamlEqual => $"{{{nameof(Builtin.YamlCompare)}({Name} = {Count})}}",
            Kind.YamlNotEqual => $"{{{nameof(Builtin.YamlCompare)}({Name} != {Count})}}",
            Kind.YamlGreaterThan => $"{{{nameof(Builtin.YamlCompare)}({Name} > {Count})}}",
            Kind.YamlGreaterThanOrEqual => $"{{{nameof(Builtin.YamlCompare)}({Name} >= {Count})}}",
            Kind.YamlLessThan => $"{{{nameof(Builtin.YamlCompare)}({Name} < {Count})}}",
            Kind.YamlLessThanOrEqual => $"{{{nameof(Builtin.YamlCompare)}({Name} <= {Count})}}",
            _ => $"{{{Name}}}",
        };
}
