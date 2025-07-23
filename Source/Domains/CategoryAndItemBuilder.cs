// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;
#pragma warning disable CA1710, IDE0028, MA0016
/// <summary>Builds the collection containing elements of <see cref="Category"/> or <see cref="Item"/>.</summary>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct CategoryAndItemBuilder : IAddTo,
    ICollection<Category>,
    ICollection<Item>,
    IComparable,
    IComparable<object>,
    IComparable<CategoryAndItemBuilder>,
    IComparisonOperators<CategoryAndItemBuilder, CategoryAndItemBuilder, bool>,
    IEquatable<object>,
    IEquatable<CategoryAndItemBuilder>
{
    /// <summary>The category builder.</summary>
    [ProvidesContext]
    readonly ImmutableArray<Category>.Builder _categories;

    /// <summary>The item builder.</summary>
    [ProvidesContext]
    readonly ImmutableArray<Item>.Builder _items;

    /// <summary>Initializes a new instance of the <see cref="CategoryAndItemBuilder"/> struct.</summary>
    public CategoryAndItemBuilder()
    {
        _items = ImmutableArray.CreateBuilder<Item>();
        _categories = ImmutableArray.CreateBuilder<Category>();
    }

    /// <summary>Initializes a new instance of the <see cref="CategoryAndItemBuilder"/> struct.</summary>
    /// <param name="category">The <see cref="Category"/> to add.</param>
    public CategoryAndItemBuilder(Category category)
    {
        _items = ImmutableArray.CreateBuilder<Item>(0);
        (_categories = ImmutableArray.CreateBuilder<Category>(1)).Add(category);
    }

    /// <summary>Initializes a new instance of the <see cref="CategoryAndItemBuilder"/> struct.</summary>
    /// <param name="item">The <see cref="Item"/> to add.</param>
    public CategoryAndItemBuilder(Item item)
    {
        _categories = ImmutableArray.CreateBuilder<Category>(0);
        (_items = ImmutableArray.CreateBuilder<Item>(1)).Add(item);
    }

    /// <summary>Gets the categories as <see cref="ArchipelagoBuilder{T}"/>.</summary>
    public ArchipelagoBuilder<Category> CategoryBuilder => new(_categories);

    /// <summary>Gets the items as <see cref="ArchipelagoBuilder{T}"/>.</summary>
    public ArchipelagoBuilder<Item> ItemBuilder => new(_items);

    /// <inheritdoc />
    [Pure]
    bool ICollection<Category>.IsReadOnly => false;

    /// <inheritdoc />
    [Pure]
    bool ICollection<Item>.IsReadOnly => false;

    /// <summary>Gets a value determining whether there are no categories in this collection.</summary>
    public bool IsCategoryEmpty => _categories.Count is 0;

    /// <summary>Gets a value determining whether there are no items in this collection.</summary>
    public bool IsItemEmpty => _items.Count is 0;

    /// <inheritdoc />
    [Pure]
    int ICollection<Category>.Count => _categories.Count;

    /// <inheritdoc />
    [Pure]
    int ICollection<Item>.Count => _items.Count;

    /// <inheritdoc />
    [Pure]
    public static bool operator ==(CategoryAndItemBuilder left, CategoryAndItemBuilder right) => left.Equals(right);

    /// <inheritdoc />
    [Pure]
    public static bool operator !=(CategoryAndItemBuilder left, CategoryAndItemBuilder right) => !left.Equals(right);

    /// <inheritdoc />
    [Pure]
    public static bool operator >(CategoryAndItemBuilder left, CategoryAndItemBuilder right) =>
        left.CompareTo(right) > 0;

    /// <inheritdoc />
    [Pure]
    public static bool operator >=(CategoryAndItemBuilder left, CategoryAndItemBuilder right) =>
        left.CompareTo(right) >= 0;

    /// <inheritdoc />
    [Pure]
    public static bool operator <(CategoryAndItemBuilder left, CategoryAndItemBuilder right) =>
        left.CompareTo(right) < 0;

    /// <inheritdoc />
    [Pure]
    public static bool operator <=(CategoryAndItemBuilder left, CategoryAndItemBuilder right) =>
        left.CompareTo(right) <= 0;

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Add"/>
    public void Add(Category item) => _categories.Add(item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Add"/>
    public void Add(Item item) => _items.Add(item);

    /// <inheritdoc />
    void ICollection<Category>.Clear() => _categories.Clear();

    /// <inheritdoc />
    void ICollection<Item>.Clear() => _items.Clear();

    /// <inheritdoc />
    public void CopyTo(Category[] array, int arrayIndex) => _categories.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public void CopyTo(Item[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    /// <inheritdoc />
    public void CopyTo(
        [NotNullIfNotNull(nameof(value))] ref JsonNode? value,
        Dictionary<string, Location>? locations,
        Dictionary<string, Region>? regions
    )
    {
        if (Json(_categories) is { } categories)
            (value ??= new JsonObject())["item_categories"] = categories;

        if (Json(_items) is { } items)
            (value ??= new JsonObject())["items"] = items;
    }

    /// <inheritdoc cref="ArchipelagoBuilder{T}.Sync(Dictionary{string, T}, Priority, bool)"/>
    public void Sync(Dictionary<string, Category> dictionary, Priority fallback = Priority.None, bool strict = true) =>
        new ArchipelagoBuilder<Category>(_categories).Sync(dictionary, fallback, strict);

    /// <inheritdoc cref="ArchipelagoBuilder{T}.Sync(Dictionary{string, T}, Priority, bool)"/>
    public void Sync(Dictionary<string, Item> dictionary, Priority fallback = Priority.None, bool strict = true) =>
        new ArchipelagoBuilder<Item>(_items).Sync(dictionary, fallback, strict);

    /// <inheritdoc />
    [Pure]
    public bool Contains(Category item) => _categories.Contains(item);

    /// <inheritdoc />
    [Pure]
    public bool Contains(Item item) => _items.Contains(item);

    /// <inheritdoc cref="object.Equals(object)"/>
    [Pure]
    public override bool Equals(object? obj) => obj is CategoryAndItemBuilder builder && Equals(builder);

    /// <inheritdoc />
    [Pure]
    public bool Equals(CategoryAndItemBuilder other)
    {
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_items is null)
            return other._items is null;

        if (_categories is null)
            return other._categories is null;

        if (other._items is null || other._categories is null)
            return false;

        // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var itemCount = _items.Count;

        if (itemCount != other._items.Count)
            return false;

        var categoryCount = _categories.Count;

        if (categoryCount != other._categories.Count)
            return false;

        for (var i = 0; i < itemCount; i++)
            if (_items[i].Name != other._items[i].Name)
                return false;

        for (var i = 0; i < categoryCount; i++)
            if (_categories[i].Name != other._categories[i].Name)
                return false;

        return true;
    }

    /// <inheritdoc />
    // ReSharper restore ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public bool Remove(Category item) => _categories.Remove(item);

    /// <inheritdoc />
    public bool Remove(Item item) => _items.Remove(item);

    /// <inheritdoc cref="IComparable.CompareTo"/>
    [Pure]
    public int CompareTo(object? other) => CompareTo(other is CategoryAndItemBuilder builder ? builder : default);

    /// <inheritdoc />
    [Pure]
    public int CompareTo(CategoryAndItemBuilder other)
    {
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_items is null)
            return other._items is null ? 0 : -1;

        if (_categories is null)
            return other._categories is null ? 0 : -1;

        if (other._items is null || other._categories is null)
            return 1;

        // ReSharper restore ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        var itemCount = _items.Count;

        if (itemCount.CompareTo(other._items.Count) is not 0 and var itemCountCompare)
            return itemCountCompare;

        var categoryCount = _categories.Count;

        if (categoryCount.CompareTo(other._categories.Count) is not 0 and var categoryCountCompare)
            return categoryCountCompare;

        for (var i = 0; i < itemCount; i++)
            if (_items[i].Name.CompareTo(other._items[i].Name) is not 0 and var itemCompare)
                return itemCompare;

        for (var i = 0; i < categoryCount; i++)
            if (_categories[i].Name.CompareTo(other._categories[i].Name) is not 0 and var categoryCompare)
                return categoryCompare;

        return 0;
    }

    /// <inheritdoc cref="object.Equals(object)"/>
    [Pure]
    public override int GetHashCode() =>
        (_categories?.AsEnumerable() ?? [])
       .Aggregate(42643801, (a, n) => unchecked(a * 37156667 ^ n.Name.GetHashCode())) ^
        (_items?.AsEnumerable() ?? []).Aggregate(32582657, (a, n) => unchecked(a * 30402457 ^ n.Name.GetHashCode()));

    /// <inheritdoc />
    [Pure]
    public override string ToString() =>
        $"[{_categories.Select(x => x.Name).Concat(_items.Select(x => x.Name)).Conjoin()}]";

    /// <inheritdoc />
    [Pure]
    public IEnumerator GetEnumerator() => _categories.Cast<object>().Concat(_items.Cast<object>()).GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator<Category> IEnumerable<Category>.GetEnumerator() => _categories.GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator<Item> IEnumerable<Item>.GetEnumerator() => _items.GetEnumerator();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.DrainToImmutable"/>
    [MustUseReturnValue]
    internal ImmutableArray<Category> ToCategories() => _categories?.ToImmutable() ?? [];

    /// <inheritdoc cref="ImmutableArray{T}.Builder.DrainToImmutable"/>
    [MustUseReturnValue]
    internal ImmutableArray<Item> ToItems() => _items?.ToImmutable() ?? [];

    /// <summary>Converts to json.</summary>
    /// <typeparam name="T">The type of collection.</typeparam>
    /// <param name="builder">The builder to convert.</param>
    /// <returns>The <see cref="JsonArray"/> of the parameter <paramref name="builder"/>.</returns>
    [Pure]
    static JsonArray? Json<T>(ImmutableArray<T>.Builder? builder)
        where T : IArchipelago<T>
    {
        if (builder is null || builder.Count is not (not 0 and var count))
            return null;

        JsonArray ret = [];

        for (var i = 0; i < count; i++)
            ret.Add(builder[i].ToString());

        return ret;
    }
}
