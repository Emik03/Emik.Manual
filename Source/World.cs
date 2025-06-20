// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;
#pragma warning disable EAM002
/// <summary>Represents the instance to build manual worlds.</summary>
/// <remarks><para>
/// Create a new instance of <see cref="World"/> and start appending with
/// <see cref="Category(Chars, ImmutableArray{Yaml})"/>, <see cref="Item"/>,
/// <see cref="Location"/>, and
/// <see cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>,
/// then use the returned value from
/// <see cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/> to call
/// <see cref="Domains.Game.WriteAsync"/>, or <see cref="Domains.Game.ZipAsync"/>.
/// </para></remarks>
/// <param name="fallback">The priority to use when one isn't specified in <see cref="Item"/>.</param>
/// <param name="strict">
/// Whether to throw an exception when an element is referred to before it is defined,
/// as oppose to implicitly creating a new one with the default settings.
/// </param>
// ReSharper disable once ClassCanBeSealed.Global ClassNeverInstantiated.Global
public partial class World(Priority fallback = Priority.Progression, bool strict = true)
{
    /// <summary>Gets the regex for the characters that are allowed.</summary>
    [StringSyntax(StringSyntaxAttribute.Regex)]
    internal const string AllowedGameChars = @"^[^\s""*/:<>?\\_|]+$", AllowedItemChars = "^[^:|]+$";

    /// <summary>Contains the generated categories.</summary>
    readonly Dictionary<string, Category> _categories = new(StringComparer.Ordinal);

    /// <summary>Contains the generated items.</summary>
    readonly Dictionary<string, Item> _items = new(StringComparer.Ordinal);

    /// <summary>Contains the generated locations.</summary>
    readonly Dictionary<string, Location> _locations = new(StringComparer.Ordinal);

    /// <summary>Contains the generated regions.</summary>
    readonly Dictionary<string, Region> _regions = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the categories that have been created so far with
    /// <see cref="Category(Chars, bool, ImmutableArray{Yaml})"/>.
    /// </summary>
    [Pure] // ReSharper disable ReturnTypeCanBeEnumerable.Global
    public ArchipelagoDictionaryValues<Category> AllCategories => new(_categories);

    /// <summary>Gets the items that have been created so far with <see cref="Item"/>.</summary>
    [Pure]
    public ArchipelagoDictionaryValues<Item> AllItems => new(_items);

    /// <summary>Gets the locations that have been created so far with <see cref="Location"/>.</summary>
    [Pure]
    public ArchipelagoDictionaryValues<Location> AllLocations => new(_locations);

    /// <summary>
    /// Gets the regions that have been created so far with
    /// <see cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>.
    /// </summary>
    [Pure]
    public ArchipelagoDictionaryValues<Region> AllRegions => new(_regions);

    /// <summary>Contains the characters not allowed in an item name.</summary>
    internal static SearchValues<char> DisallowedItemChars { get; } = SearchValues.Create(":|");

    /// <inheritdoc />
    [Pure] // ReSharper restore ReturnTypeCanBeEnumerable.Global
    public override string ToString() =>
        $"{_categories.Count.Conjugate("Category", "-ies")
        }, {_items.Count.Conjugate("Item")
        }, {_locations.Count.Conjugate("Location")
        }, {_regions.Count.Conjugate("Region")}";

    /// <inheritdoc cref="Category(Chars, bool, ImmutableArray{Yaml})"/>
    public ref readonly Category Category(Chars name) => ref Category(name, false, default);

    /// <inheritdoc cref="Category(Chars, bool, ImmutableArray{Yaml})"/>
    public ref readonly Category Category(Chars name, params ImmutableArray<Yaml> yaml) =>
        ref Category(name, false, yaml);

    /// <inheritdoc cref="Category(Chars, bool, ImmutableArray{Yaml})"/>
    public ref readonly Category Category(Chars name, bool isHidden) => ref Category(name, isHidden, []);

    /// <summary>Adds or gets the existing category.</summary>
    /// <remarks><para>Arguments after the first are ignored when an item with the same name exists.</para></remarks>
    /// <param name="name">The name of the category.</param>
    /// <param name="isHidden">Whether to hide this category in the GUI.</param>
    /// <param name="yaml">
    /// If populated, any yaml options that needs to be true for this category to exist.
    /// </param>
    /// <returns>The reference to the category entry.</returns>
    /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/categories-for-items-and-locations.md"/>
    public ref readonly Category Category(Chars name, bool isHidden = false, params ImmutableArray<Yaml> yaml)
    {
        ref var category = ref Ref(_categories, name, out var exists);

        if (exists)
            return ref category;

        category = new(name, isHidden, yaml.IsDefault ? [] : yaml);
        return ref category;
    }

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game() => Game(default, default, default, false, 1, default);

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game(params ImmutableArray<StartingItemBlock> startingItems) =>
        Game(default, default, default, false, 1, startingItems);

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game([Match(AllowedGameChars, true)] Chars name) => Game(name, default, default, false, 1, default);

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game(
        [Match(AllowedGameChars, true)] Chars name,
        params ImmutableArray<StartingItemBlock> startingItems
    ) =>
        Game(name, default, default, false, 1, startingItems);

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game(
        [Match(AllowedGameChars, true)] Chars name,
        Chars creator = default,
        params ImmutableArray<StartingItemBlock> startingItems
    ) =>
        Game(name, creator, default, false, 1, startingItems);

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game(
        [Match(AllowedGameChars, true)] Chars name,
        [Match(AllowedGameChars, true)] Chars creator = default,
        Chars filler = default,
        params ImmutableArray<StartingItemBlock> startingItems
    ) =>
        Game(name, creator, filler, false, 1, startingItems);

    /// <inheritdoc cref="Game(Chars, Chars, Chars, bool, int, ImmutableArray{StartingItemBlock})"/>"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game(
        [Match(AllowedGameChars, true)] Chars name,
        [Match(AllowedGameChars, true)] Chars creator = default,
        Chars filler = default,
        bool deathLink = false,
        params ImmutableArray<StartingItemBlock> startingItems
    ) =>
        Game(name, creator, filler, deathLink, 1, startingItems);

    /// <summary>Serializes and writes <c>json</c> files to disk.</summary>
    /// <param name="name">The name of your game, compatible with capital letters.</param>
    /// <param name="creator">Your Username, by default falls back to your system name.</param>
    /// <param name="filler">
    /// Name of the filler items that get placed when there's no more real items to place.
    /// </param>
    /// <param name="deathLink">Does your game support Death Link?</param>
    /// <param name="startingIndex">Choose the starting index for your locations and items.</param>
    /// <param name="startingItems">Starting inventory</param>
    /// <exception cref="KeyNotFoundException">
    /// This parameter <paramref name="startingItems"/> refer to an element not yet defined,
    /// and the parameter <see name="strict"/> is <see langword="true"/>.
    /// </exception>
    /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/game.md"/>
    [MustUseReturnValue("Use Game.WriteAsync() or Game.ZipAsync() to generate the manual world.")]
    public Game Game(
        [Match(AllowedGameChars, true)] Chars name,
        [Match(AllowedGameChars, true)] Chars creator = default,
        Chars filler = default,
        bool deathLink = false,
        int startingIndex = 1,
        params ImmutableArray<StartingItemBlock> startingItems
    )
    {
        foreach (var startingItem in startingItems = startingItems.IsDefault ? [] : startingItems)
        {
            startingItem.Set.Sync(_items, fallback, strict);
            startingItem.DependsOn.Sync(_items, fallback, strict);
        }

        var regex = DisallowedGameChars();

        return new(
            this,
            name.Replace(regex, strict),
            creator.Replace(regex, strict),
            filler,
            deathLink,
            startingIndex,
            startingItems
        );
    }

    /// <summary>
    /// Gets the items created so far with <see cref="Item"/> that contains the category listed.
    /// </summary>
    /// <param name="category">The category to match against.</param>
    /// <returns>
    /// The filtered collection of <see cref="Item"/> instances that match the parameter <paramref name="category"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public IEnumerable<Item> AllItemsWith(Category category)
    {
        ArchipelagoListBuilder<Category>.Sync(_categories, ref category, fallback, strict);
        return _items.Values.Where(x => x.Categories.Any(Match(category)));
    }

    /// <summary>
    /// Gets the items created so far with <see cref="Item"/> that contains any category listed.
    /// </summary>
    /// <param name="categories">The categories to match against.</param>
    /// <returns>
    /// The filtered collection of <see cref="Item"/> instances that
    /// match any value in the parameter <paramref name="categories"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public IEnumerable<Item> AllItemsWithAny(params ArchipelagoListBuilder<Category> categories)
    {
        categories.Sync(_categories, fallback, strict);
        return _items.Values.Where(x => x.Categories.Any(Match(categories)));
    }

    /// <summary>
    /// Gets the locations created so far with <see cref="Item"/> that contains all categories listed.
    /// </summary>
    /// <param name="categories">The categories to match against.</param>
    /// <returns>
    /// The filtered collection of <see cref="Item"/> instances that
    /// match all values in the parameter <paramref name="categories"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public IEnumerable<Item> AllItemsWithAll(params ArchipelagoListBuilder<Category> categories)
    {
        categories.Sync(_categories, fallback, strict);
        return _items.Values.Where(x => x.Categories.All(Match(categories)));
    }

    /// <summary>
    /// Gets the locations created so far with <see cref="Location"/> that contains the category listed.
    /// </summary>
    /// <param name="category">The category to match against.</param>
    /// <returns>
    /// The filtered collection of <see cref="Location"/> instances
    /// that match the parameter <paramref name="category"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public IEnumerable<Location> AllLocationsWith(Category category)
    {
        ArchipelagoListBuilder<Category>.Sync(_categories, ref category, fallback, strict);
        return _locations.Values.Where(x => x.Categories.Any(Match(category)));
    }

    /// <summary>
    /// Gets the locations created so far with <see cref="Location"/> that contains any category listed.
    /// </summary>
    /// <param name="categories">The categories to match against.</param>
    /// <returns>
    /// The filtered collection of <see cref="Location"/> instances that
    /// match any value in the parameter <paramref name="categories"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public IEnumerable<Location> AllLocationsWithAny(params ArchipelagoListBuilder<Category> categories)
    {
        categories.Sync(_categories, fallback, strict);
        return _locations.Values.Where(x => x.Categories.Any(Match(categories)));
    }

    /// <summary>
    /// Gets the locations created so far with <see cref="Location"/> that contains all categories listed.
    /// </summary>
    /// <param name="categories">The categories to match against.</param>
    /// <returns>
    /// The filtered collection of <see cref="Location"/> instances that
    /// match all values in the parameter <paramref name="categories"/>.
    /// </returns>
    [LinqTunnel, Pure]
    public IEnumerable<Location> AllLocationsWithAll(params ArchipelagoListBuilder<Category> categories)
    {
        categories.Sync(_categories, fallback, strict);
        return _locations.Values.Where(x => x.Categories.All(Match(categories)));
    }

    /// <summary>Adds or gets the existing item.</summary>
    /// <remarks><para>Arguments after the first are ignored when an item with the same name exists.</para></remarks>
    /// <param name="name">The name of the item.</param>
    /// <param name="priority">The importance and placement bias of the item.</param>
    /// <param name="categories">A list of categories to be applied to this item.</param>
    /// <param name="count">Total number of this item that will be in the item pool for randomization.</param>
    /// <param name="giveItems">A dictionary of values this item has. Used for <see cref="Builtin.ItemValue"/>.</param>
    /// <param name="early">
    /// How many copies of this item are required to be placed somewhere accessible from the start.
    /// </param>
    /// <param name="localEarly"></param>
    /// <param name="id"></param>
    /// <returns>The reference to the item entry.</returns>
    /// <exception cref="KeyNotFoundException">
    /// This parameter <paramref name="categories"/> refer to an element not yet defined,
    /// and the parameter <see name="strict"/> is <see langword="true"/>.
    /// </exception>
    /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/items.md"/>
    public ref readonly Item Item(
        [Match(AllowedItemChars, true)] Chars name,
        Priority? priority = null,
        [InstantHandle] ArchipelagoListBuilder<Category> categories = default,
        int count = 1,
        ImmutableArray<(Chars PhantomItemName, int Amount)> giveItems = default,
        int early = 0,
        int localEarly = 0,
        int? id = null
    )
    {
        ref var item = ref Ref(_items, name, out var exists);

        if (exists)
            return ref item;

        categories.Sync(_categories, fallback, strict);

        item = new(
            name.Replace(DisallowedItemChars, strict),
            priority ?? fallback,
            categories.ToImmutable(),
            count,
            giveItems,
            early,
            localEarly,
            id
        );

        return ref item;
    }

    /// <summary>Adds or gets the existing location.</summary>
    /// <remarks><para>Arguments after the first are ignored when an item with the same name exists.</para></remarks>
    /// <param name="name">The unique name of the location.</param>
    /// <param name="logic">
    /// A boolean logic object that describes the required items, counts, etc. needed to reach this location.
    /// </param>
    /// <param name="categories">A list of categories to be applied to this location.</param>
    /// <param name="region">The name of the region this location is part of.</param>
    /// <param name="options">Additional boolean configurations for this location.</param>
    /// <param name="allowList">
    /// Places an item that matches one of the item names or at least one of the categories listed
    /// in this setting at this location. Does not check logical access to the location.
    /// </param>
    /// <param name="denyList">
    /// Configures what item names or categories should not end up at this location during
    /// normal generation. Does not check logical access to the location.
    /// </param>
    /// <param name="hintEntrance">
    /// Adds additional text to this location's hints to convey useful
    /// information. Typically used for entrance randomization.
    /// </param>
    /// <param name="id">
    /// Skips the item ID forward to the given value. This can be used to provide buffer space for future items.
    /// </param>
    /// <returns>The reference to the location entry.</returns>
    /// <exception cref="KeyNotFoundException">
    /// This parameters <paramref name="logic"/>, <paramref name="categories"/>, <paramref name="allowList"/>,
    /// or <paramref name="denyList"/>, refer to an element not yet defined, and the parameter <see name="strict"/>
    /// is <see langword="true"/>.
    /// </exception>
    /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/locations.md"/>
    public ref readonly Location Location(
        Chars name,
        Logic? logic = null,
        [InstantHandle] ArchipelagoListBuilder<Category> categories = default,
        scoped in Region region = default,
        LocationOptions options = LocationOptions.None,
        [InstantHandle] CategoryOrItemListBuilder allowList = default,
        [InstantHandle] CategoryOrItemListBuilder denyList = default,
        Chars hintEntrance = default,
        int? id = null
    )
    {
        ref var location = ref Ref(_locations, name, out var exists);

        if (exists)
            return ref location;

        denyList.Sync(_items, fallback, strict);
        allowList.Sync(_items, fallback, strict);
        categories.Sync(_categories, fallback, strict);
        logic?.ThrowIfUnreferenced(_categories, _items, strict);

        location = new(
            name,
            logic,
            categories.ToImmutable(),
            region,
            options,
            denyList.ToCategories(),
            allowList.ToCategories(),
            allowList.ToItems(),
            denyList.ToItems(),
            hintEntrance,
            id
        );

        return ref location;
    }

    /// <inheritdoc cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>
    public ref readonly Region Region(Chars name) => ref Region(name, null, default(ArchipelagoListBuilder<Region>));

    /// <inheritdoc cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>
    public ref readonly Region Region(Chars name, Logic? logic) =>
        ref Region(name, logic, default(ArchipelagoListBuilder<Region>));

    /// <inheritdoc cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>
    // ReSharper disable MethodOverloadWithOptionalParameter
    public ref readonly Region Region(Chars name, Logic? logic, bool isStarting) =>
        ref Region(name, logic, default(ArchipelagoListBuilder<Region>), isStarting);

    /// <inheritdoc cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>
    // ReSharper disable MethodOverloadWithOptionalParameter
    public ref readonly Region Region(Chars name, bool isStarting) =>
        ref Region(name, null, default(ArchipelagoListBuilder<Region>), isStarting);

    /// <inheritdoc cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>
    // ReSharper disable MethodOverloadWithOptionalParameter
    public ref readonly Region Region(
        Chars name,
        Logic? logic = null,
        bool isStarting = false,
        [InstantHandle] ArchipelagoListBuilder<Passage> entrances = default
    ) =>
        ref Region(name, logic, default(ArchipelagoListBuilder<Region>), isStarting, entrances);

    /// <summary>Adds or gets the existing region.</summary>
    /// <param name="name">Name of the region.</param>
    /// <param name="logic">
    /// The boolean logic object that describes the required items, counts, etc. needed to reach this region.
    /// </param>
    /// <param name="connectsTo">
    /// A list of other regions that the player can reach from this region. Only describe forward connections
    /// with this setting, as backward connections are implied from regions you have already accessed.
    /// </param>
    /// <param name="isStarting">
    /// Is this region accessible from the start? Or, does this region not require a connection from another region first?
    /// </param>
    /// <param name="entrances">
    /// Additional requirements to use a specific connection into this region.
    /// </param>
    /// <returns>The reference to the region entry.</returns>
    /// <exception cref="KeyNotFoundException">
    /// This parameters <paramref name="logic"/>, <paramref name="connectsTo"/>, or <paramref name="entrances"/>,
    /// refer to an element not yet defined, and the parameter <see name="strict"/>
    /// is <see langword="true"/>.
    /// </exception>
    /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/regions.md"/>
    public ref readonly Region Region(
        Chars name,
        Logic? logic = null,
        [InstantHandle] ArchipelagoListBuilder<Region> connectsTo = default,
        bool isStarting = false,
        [InstantHandle] ArchipelagoListBuilder<Passage> entrances = default
    )
    {
        ref var region = ref Ref(_regions, name, out var exists);

        if (exists)
            return ref region;

        logic?.ThrowIfUnreferenced(_categories, _items, strict);
        connectsTo.Sync(_regions, fallback, strict);
        entrances.Sync(_categories, _items, _regions, fallback, strict);
        region = new(name, logic, connectsTo.ToImmutable(), isStarting, entrances.ToImmutable(), []);
        return ref region;
    }

    /// <inheritdoc cref="Region(Chars, Logic, ArchipelagoListBuilder{Domains.Region}, bool, ArchipelagoListBuilder{Passage})"/>
    public ref readonly Region Region(
        Chars name,
        Logic? logic = null,
        [InstantHandle] ArchipelagoListBuilder<Passage> connectsTo = default,
        bool isStarting = false,
        [InstantHandle] ArchipelagoListBuilder<Passage> entrances = default
    )
    {
        ref var region = ref Ref(_regions, name, out var exists);

        if (exists)
            return ref region;

        logic?.ThrowIfUnreferenced(_categories, _items, strict);
        connectsTo.Sync(_categories, _items, _regions, fallback, strict);
        entrances.Sync(_categories, _items, _regions, fallback, strict);
        var exits = connectsTo.ToImmutable();

        region = new(
            name,
            logic,
            ImmutableArray.CreateRange(exits, x => x.Region),
            isStarting,
            entrances.ToImmutable(),
            exits
        );

        return ref region;
    }

    /// <summary>Creates the <see cref="string"/> for <c>options.json</c>.</summary>
    /// <returns>The <see cref="string"/>.</returns>
    internal string? OptionsString() =>
        Options()?.ToJsonString(new() { TypeInfoResolver = new DefaultJsonTypeInfoResolver(), WriteIndented = true });

    /// <summary>Creates the yaml comments showcasing all of a particular value.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="all">All values, without any filtering.</param>
    /// <param name="allWith">The function to get all values of a specific category.</param>
    /// <param name="exportedCount">The exported count.</param>
    /// <param name="totalCount">The total count.</param>
    /// <param name="header">The header.</param>
    /// <param name="listAll">Whether to return the representation, or an empty <see langword="string"/>.</param>
    /// <returns>The yaml comments.</returns>
    [Pure]
    internal string Show<T>(
        ArchipelagoDictionaryValues<T> all,
        Converter<Category, IEnumerable<T>> allWith,
        int exportedCount,
        int totalCount,
        string header,
        bool listAll
    )
        where T : IArchipelago<T> =>
        listAll &&
        AllCategories
           .Where(x => !x.IsHidden)
           .Select(x => (x.Name, Locations: (IList<T>)[..allWith(x)]))
           .Prepend(("(No Category)", (IList<T>)[..all.Where(x => x.Categories.IsDefaultOrEmpty)]))
           .Where(x => x.Locations is not [])
           .Select(Show)
           .Concat() is var values
            ? $"""

                 # {header} ({totalCount} total, {exportedCount} potentially non-local):
               {values}
               """
            : "";

    /// <summary>Creates the <see cref="string"/> for <c>options.json</c>.</summary>
    /// <returns>The <see cref="string"/>.</returns>
    internal JsonObject? Options()
    {
        (bool IsToggle, string Yaml) Deconstruct((Builtin Context, Yaml Yaml) x) =>
            (IsToggle: x.Context is Builtin.YamlDisabled or Builtin.YamlEnabled, Yaml: x.Yaml.ToString(strict));

        static (JsonObject Object, string Yaml) CreateJsonObject((bool IsToggle, string Yaml) x)
        {
            JsonObject obj = new()
            {
                ["description"] = new JsonArray(),
                ["type"] = x.IsToggle ? "Toggle" : "Range",
                ["default"] = x.IsToggle ? false : 0,
            };

            if (x.IsToggle)
                return (obj, x.Yaml);

            obj["range_start"] = 0;
            obj["range_end"] = 10;
            obj["values"] = new JsonObject();
            return (obj, x.Yaml);
        }

        JsonObject obj = [];

        bool CanAdd((bool IsToggle, string Yaml) x) =>
            !obj.TryGetPropertyValue(x.Yaml, out var node) || // ReSharper disable once NullableWarningSuppressionIsUsed
            ((JsonObject)node!).ContainsKey("values") && x.IsToggle;

        AllLocations.SelectMany(x => x.Logic?.YamlSettings ?? [])
           .Concat(AllRegions.SelectMany(x => x.Logic?.YamlSettings ?? []))
           .Select(Deconstruct)
           .Where(CanAdd)
           .Select(CreateJsonObject)
           .Lazily(x => obj[x.Yaml] = x.Object)
           .Enumerate();

        return obj.Count is 0 ? null : new JsonObject { ["user"] = obj };
    }

    /// <summary>Creates the yaml comment displaying this value.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="tuple">The tuple to display.</param>
    /// <returns>The yaml comment.</returns>
    [Pure]
    static string Show<T>((Chars Name, IList<T> Values) tuple)
        where T : IArchipelago<T> =>
        $"""
           #   {tuple.Name}{(tuple.Values.Count is not 1 && tuple.Values.Sum(x => x.Count) is not 1 and var count
               ? $" ({count})" : "")}:
         {tuple.Values.Select(Show).Concat()}
         """;

    /// <summary>Creates the yaml comment displaying this value.</summary>
    /// <typeparam name="T">The type of value.</typeparam>
    /// <param name="value">The value to display.</param>
    /// <returns>The yaml comment.</returns>
    [Pure]
    static string Show<T>(T value)
        where T : IArchipelago<T> =>
        $"""
           #     - {(value.Name.Span.Contains(':') ? $"\"{value.Name.Span}\"" : value.Name.Span)}{
               (value.Count is not 1 and var count ? $": {count}" : "")}

         """;

    /// <summary>Gives the function that evaluates whether the category matches the parameter.</summary>
    /// <param name="item">The <see cref="Domains.Category"/> containing what to match.</param>
    /// <returns>The function that tests for the <see cref="Domains.Category"/>.</returns>
    static Func<Category, bool> Match(Category item) =>
        x => item.Name.Span.Equals(x.Name.Span, StringComparison.Ordinal);

    /// <summary>Gives the function that evaluates whether the category matches any values in the parameter.</summary>
    /// <param name="builder">The <see cref="ImmutableArray{T}"/> containing what to match.</param>
    /// <returns>The function that tests for the <see cref="Domains.Category"/>.</returns>
    static Func<Category, bool> Match(ArchipelagoListBuilder<Category> builder) =>
        x => builder.Any(y => y.Name.Span.Equals(x.Name.Span, StringComparison.Ordinal));

    /// <summary>
    /// Wrapper around <see cref="CollectionsMarshal.GetValueRefOrAddDefault{TKey, TValue, TAlternateKey}"/>.
    /// </summary>
    /// <typeparam name="T">The value of the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to get or add.</param>
    /// <param name="name">The key.</param>
    /// <param name="exists">Whether the value already existed.</param>
    /// <returns>The reference to the element.</returns>
    static ref T? Ref<T>(Dictionary<string, T> dictionary, Chars name, out bool exists) =>
        ref CollectionsMarshal.GetValueRefOrAddDefault(
            dictionary.GetAlternateLookup<ReadOnlySpan<char>>(),
            name.Span,
            out exists
        );

    [GeneratedRegex(@"[\s""*/:<>?\\_|]")]
    private static partial Regex DisallowedGameChars();
}
