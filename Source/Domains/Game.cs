// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents the <c>game.json</c> file.</summary>
/// <param name="World">Contains all categories, items, locations, and regions.</param>
/// <param name="Name">The name of your game, compatible with capital letters.</param>
/// <param name="Creator">Your Username, by default falls back to your system name.</param>
/// <param name="Filler">
/// Name of the filler items that get placed when there's no more real items to place.
/// </param>
/// <param name="DeathLink">Does your game support Death Link?</param>
/// <param name="StartingIndex">Choose the starting index for your locations and items.</param>
/// <param name="StartingItems">The starting inventory.</param>
/// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/game.md"/>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct Game(
    World? World,
    Chars Name,
    Chars Creator = default,
    Chars Filler = default,
    bool DeathLink = false,
    int StartingIndex = 1,
    params ImmutableArray<StartingItemBlock> StartingItems
) : IAddTo, IArchipelago<Game>
{
    /// <inheritdoc />
    [Pure]
    public static implicit operator Game(string? name) => new(null, name.AsMemory());

    /// <inheritdoc />
    [Pure]
    public static implicit operator Game(ReadOnlyMemory<char> name) => new(null, name);

    /// <inheritdoc />
    void IAddTo.CopyTo([NotNullIfNotNull(nameof(value))] ref JsonNode? value, IReadOnlyCollection<Region>? regions)
    {
        value ??= new JsonObject
        {
            ["game"] = GameName(),
            ["filler_item_name"] = Filler.IsEmpty ? "(filler)" : Filler.ToString(),
            ["creator"] = Creator.IsEmpty ? Environment.UserName : Creator.ToString(),
            ["_comment"] = "Handcrafted with <3 and pain, using Emik.Manual: https://github.com/Emik03/Emik.Manual",
        };

        if (DeathLink)
            value["death_link"] = true;

        if (StartingIndex is not 1)
            value["starting_index"] = StartingIndex;

        if (StartingItems.IsDefaultOrEmpty)
            return;

        JsonNode? node = null;

        foreach (var startingItem in StartingItems)
            startingItem.CopyTo(ref node, regions);

        if (node is not null)
            value["starting_items"] = node;
    }

    /// <summary>Gets the current number of items, excluding ones found in <see cref="StartingItems"/>.</summary>
    [Pure]
    public int EffectiveItemCount() =>
        World is not null ? World.AllItems.Sum(x => x.Count) - StartingItems.Sum(Min) : 0;

    /// <summary>Gets the current number of items that can appear in other worlds.</summary>
    [Pure]
    public int ExportedItemCount() =>
        World is not null ? World.AllItems.Sum(CountExported) - StartingItems.Sum(Min) : 0;

    /// <summary>Gets the current number of locations.</summary>
    [Pure]
    public int EffectiveLocationCount() => World?.AllLocations.Count ?? 0;

    /// <summary>Gets the current number of locations that can give an item that belongs in another world.</summary>
    [Pure]
    public int ExportedLocationCount() =>
        World?.AllLocations.Count(x => x.ItemAllowList.IsDefaultOrEmpty && x.CategoryAllowList.IsDefaultOrEmpty) ?? 0;

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Name.ToString();

    /// <summary>Creates the yaml template.</summary>
    /// <param name="listChecks">Whether to list all items and locations.</param>
    /// <returns>The yaml template.</returns>
    [Pure]
#pragma warning disable MA0051
    public string Template(bool listChecks = true)
#pragma warning restore MA0051
    {
        var world = World;
        var name = FullName();

        var items = world?.Show(
            world.AllItems,
            world.AllItemsWith,
            EffectiveItemCount(),
            world.AllItems.Sum(x => x.Count),
            "Items",
            listChecks
        );

        var locations = world?.Show(
            world.AllLocations,
            world.AllLocationsWith,
            ExportedLocationCount(),
            world.AllLocations.Count,
            "Locations",
            listChecks
        );

        var yaml = world?.Options() is { } obj // ReSharper disable once NullableWarningSuppressionIsUsed
            ? ((JsonObject)obj["user"]!).Select(Show).Append("\n").Prepend("  # Custom Settings:\n").Concat()
            : "";

        var goal = world is not null &&
            (ICollection<Location>)[..world.AllLocations.Where(x => x.Options.Has(LocationOptions.Victory))] is var col
                ? Goal(col)
                : "";

        return $$"""
              # Q. What is this file?
              # A. This file contains options which allow you to configure your multiworld experience while allowing
              #    others to play how they want as well.
              #
              # Q. How do I use it?
              # A. The options in this file are weighted. This means the higher number you assign to a value, the
              #    more chances you have for that option to be chosen. For example, an option like this:
              #
              #    map_shuffle:
              #      on: 5
              #      off: 15
              #
              #    Means you have 5 chances for map shuffle to occur, and 15 chances for map shuffle to be turned
              #    off.
              #
              # Q. I've never seen a file like this before. What characters am I allowed to use?
              # A. This is a .yaml file. You are allowed to use most characters.
              #    To test if your yaml is valid or not, you can use this website:
              #        http://www.yamllint.com/
              #    You can also verify that your Archipelago options are valid at this site:
              #        https://archipelago.gg/check

              # Your name in-game, limited to 16 characters.
              #     {player} will be replaced with the player's slot number.
              #     {PLAYER} will be replaced with the player's slot number, if that slot number is greater than 1.
              #     {number} will be replaced with the counter value of the name.
              #     {NUMBER} will be replaced with the counter value of the name, if the counter value is greater than 1.
              name: Player{number}

              # Used to describe your yaml. Useful if you have multiple files.
              description: Default {{GameName()}} Template

              game: {{name}}

              {{name}}:
                # Game Options
                progression_balancing:
                  # A system that can move progression earlier, to try and prevent the player from getting stuck and bored early.
                  # 
                  # A lower setting means more getting stuck. A higher setting means less getting stuck.
                  #
                  # You can define additional values between the minimum and maximum values.
                  # Minimum value is 0
                  # Maximum value is 99
                  random: 0
                  random-low: 0
                  random-high: 0
                  disabled: 0 # equivalent to 0
                  normal: 50 # equivalent to 50
                  extreme: 0 # equivalent to 99

                accessibility:
                  # Set rules for reachability of your items/locations.
                  # 
                  # **Full:** ensure everything can be reached and acquired.
                  # 
                  # **Minimal:** ensure what is needed to reach your goal can be acquired.
                  full: 50
                  minimal: 0

              {{goal}}{{yaml}}  # Item & Location Options
                local_items:
                  # Forces these items to be in their native world.
                  []

                non_local_items:
                  # Forces these items to be outside their native world.
                  []

                start_inventory:
                  # Start with these items.
                  {}

                start_hints:
                  # Start with these item's locations prefilled into the ``!hint`` command.
                  []

                start_location_hints:
                  # Start with these locations and their item prefilled into the ``!hint`` command.
                  []

                exclude_locations:
                  # Prevent these locations from having an important item.
                  []

                priority_locations:
                  # Prevent these locations from having an unimportant item.
                  []

                item_links:
                  # Share part of your item pool with other players.
                  []
              {{locations}}{{items}}
              """;
    }

    /// <summary>Displays effective location and item counts to the passed function.</summary>
    /// <param name="action">The function to invoke, or <see cref="Console.WriteLine(string)"/>.</param>
    /// <returns>Itself.</returns>
    public Game DisplayEffective(Action<string>? action = null)
    {
        (action ?? Console.WriteLine).Invoke($"Effective: {EffectiveLocationCount()}/{EffectiveItemCount()}");
        return this;
    }

    /// <summary>Displays exported location and item counts to the passed function.</summary>
    /// <param name="action">The function to invoke, or <see cref="Console.WriteLine(string)"/>.</param>
    /// <returns>Itself.</returns>
    public Game DisplayExported(Action<string>? action = null)
    {
        (action ?? Console.WriteLine).Invoke($"Exported: {ExportedLocationCount()}/{ExportedItemCount()}");
        return this;
    }

    /// <summary>Writes the <c>.json</c> files to disk.</summary>
    /// <param name="directory">The directory to write to.</param>
    /// <param name="listChecks">
    /// When specified, additionally writes the template <c>.yaml</c>, passing this parameter in <see cref="Template"/>.
    /// </param>
    /// <param name="expandLocations">
    /// Whether to replace all uses of <see cref="Builtin.CanReachLocation"/> by expanding it into the equivalent
    /// logic, since use of <see cref="Builtin.CanReachLocation"/> can dramatically increase generation time.
    /// Expansion will make this method take longer, and can make the resulting logic strings difficult to read.
    /// </param>
    /// <param name="token">The cancellation token.</param>
    /// <exception cref="IOException">A file could not be written.</exception>
    public Task WriteAsync(
        string? directory = null,
        bool? listChecks = true,
        bool expandLocations = false,
        CancellationToken token = default
    )
    {
        Directory.CreateDirectory(directory ??= Path.Join(Environment.CurrentDirectory, "data"));
        var regions = expandLocations ? World?.AllRegions.Values.WithCount(World.AllRegions.Count) : null;
        var game = DiskAsync(Path.Join(directory, "game.json"), Json([this], regions), token);

        return World is null
            ? game
            : Task.WhenAll(
                DiskAsync(Path.Join(directory, $"{FullName()}.yaml"), listChecks is { } ls ? Template(ls) : "", token),
                DiskAsync(Path.Join(directory, "categories.json"), Json(World.AllCategories, regions), token),
                DiskAsync(Path.Join(directory, "locations.json"), Json(World.AllLocations, regions), token),
                DiskAsync(Path.Join(directory, "regions.json"), Json(World.AllRegions, regions), token),
                DiskAsync(Path.Join(directory, "options.json"), World.OptionsString(), token),
                DiskAsync(Path.Join(directory, "items.json"), Json(World.AllItems, regions), token),
                game
            );
    }

    /// <summary>Writes the <c>.apworld</c> file to disk.</summary>
    /// <param name="destination">The destination path. If this value is a directory, the file name is inferred.</param>
    /// <param name="apWorld">
    /// The path to the local template <c>.apworld</c>. If <see langword="null"/>, pulls the latest version from GitHub.
    /// </param>
    /// <param name="listChecks">
    /// When specified, writes the template <c>.yaml</c> in the same directory as the parameter
    /// <paramref name="destination"/>, passing this parameter in <see cref="Template"/>.
    /// </param>
    /// <param name="expandLocations">
    /// Whether to replace all uses of <see cref="Builtin.CanReachLocation"/> by expanding it into the equivalent
    /// logic, since use of <see cref="Builtin.CanReachLocation"/> can dramatically increase generation time.
    /// Expansion will make this method take longer, and can make the resulting logic strings difficult to read.
    /// </param>
    /// <param name="token">The cancellation token.</param>
    /// <exception cref="ArgumentException">The zip file could not be opened.</exception>
    /// <exception cref="IOException">The file could not be written.</exception>
    /// <exception cref="InvalidOperationException">The data from the API is in an unrecognized format.</exception>
    public async Task ZipAsync(
        string destination,
        string? apWorld = null,
        bool? listChecks = null,
        bool expandLocations = false,
        CancellationToken token = default
    )
    {
        var name = FullName();

        async Task CopyToAsync(ZipArchive zip, string path, string? json, CancellationToken token)
        {
            if (json is null)
                return;

            var rent = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(json));
            await using MemoryStream reader = new(rent, 0, Encoding.UTF8.GetBytes(json, rent), false);
            await using var writer = zip.CreateEntry($"{name}/{path}").Open();
            await reader.CopyToAsync(writer, token);
            ArrayPool<byte>.Shared.Return(rent);
        }

        if (Directory.Exists(destination))
            destination = Path.Join(destination, $"{name}.apworld");

        using ZipArchive reader = new(await Reader(apWorld, token), ZipArchiveMode.Read),
            writer = new(new FileStream(destination, FileMode.Create, FileAccess.Write), ZipArchiveMode.Create);

        var regions = expandLocations ? World?.AllRegions.Values.WithCount(World.AllRegions.Count) : null;

        foreach (var entry in reader.Entries)
            if (entry.FullName is not [.., '/'] and not [.., '.', 'j', 's', 'o', 'n'] &&
                entry.FullName.IndexOf('/') is not -1 and var index &&
                entry.FullName[(index + 1)..] is var e)
            {
                await using var entryReader = entry.Open();
                await using var entryWriter = writer.CreateEntry($"{name}/{e}").Open();
                await entryReader.CopyToAsync(entryWriter, token);
            }
#pragma warning disable CA2025
        var game = CopyToAsync(writer, "data/game.json", Json([this], regions), token);
#pragma warning restore CA2025
        await (World is null
            ? game
            : Task.WhenAll(
                DiskAsync(
                    Path.Join(Path.GetDirectoryName(destination), $"{FullName()}.yaml"),
                    listChecks is { } lc ? Template(lc) : null,
                    token
                ),
                CopyToAsync(writer, "data/categories.json", Json(World.AllCategories, regions), token),
                CopyToAsync(writer, "data/locations.json", Json(World.AllLocations, regions), token),
                CopyToAsync(writer, "data/regions.json", Json(World.AllRegions, regions), token),
                CopyToAsync(writer, "data/options.json", World.OptionsString(), token),
                CopyToAsync(writer, "data/items.json", Json(World.AllItems, regions), token),
                game
            ));
    }

    /// <summary>Gets the minimum <see cref="Item.Count"/>.</summary>
    /// <param name="items">The items to check.</param>
    /// <returns>The minimum <see cref="Item.Count"/>.</returns>
    [Pure]
    static int Min(ArchipelagoListBuilder<Item> items) => items.Aggregate(int.MaxValue, (a, n) => a.Min(n.Count));

    /// <summary>Creates the yaml goal parameter.</summary>
    /// <param name="goals">The list of locations that can be goaled.</param>
    /// <returns>The goal yaml setting.</returns>
    [Pure]
    static string Goal(ICollection<Location> goals) =>
        goals.Count <= 1
            ? ""
            : $"""
                 goal:
                   # The goal required of you in order to complete your run in Archipelago.
               {goals.Select((x, i) => $"    {x}: {(i is 0 ? 50 : 0)}\n").Concat()}

               """;

    /// <summary>Creates the <c>JSON</c> <see cref="string"/> out of the collection.</summary>
    /// <typeparam name="T">The type of collection to enumerate and serialize.</typeparam>
    /// <param name="elements">The collection to enumerate and serialize.</param>
    /// <param name="regions">The regions.</param>
    /// <returns>The serialized <c>JSON</c> <see cref="string"/> of the parameter <paramref name="elements"/>.</returns>
    [Pure]
    static string? Json<T>(IEnumerable<T>? elements, IReadOnlyCollection<Region>? regions)
        where T : IAddTo, IArchipelago<T>
    {
        if (elements is null)
            return null;

        JsonNode? t = null;

        foreach (var value in elements)
            value.CopyTo(ref t, regions);

        return t?.ToJsonString(new() { TypeInfoResolver = new DefaultJsonTypeInfoResolver(), WriteIndented = true });
    }

    /// <summary>Displays this yaml setting.</summary>
    /// <param name="pair">The key-value pair from the <see cref="JsonObject"/>.</param>
    /// <returns>The yaml setting</returns>
    [Pure]
    static string Show(KeyValuePair<string, JsonNode?> pair) =>
        // ReSharper disable once NullableWarningSuppressionIsUsed
        $"  {pair.Key}: {(((JsonObject)pair.Value!).ContainsKey("values") ? "0" : "false")}\n";

    /// <summary>Writes to disk asynchronously if text is provided.</summary>
    /// <param name="file">The file to write.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The task.</returns>
    [Pure]
    static Task DiskAsync(string file, string? text, CancellationToken token) =>
        text is not null ? File.WriteAllTextAsync(file, text, token) : Task.CompletedTask;

    /// <summary>Gets the <see cref="Stream"/> of the latest <c>.apworld</c> from GitHub.</summary>
    /// <param name="apWorld">
    /// The path to the local template <c>.apworld</c>. If <see langword="null"/>, pulls the latest version from GitHub.
    /// </param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>The latest <c>.apworld</c> from GitHub as an <see cref="ZipArchive"/> instance.</returns>
    /// <exception cref="ArgumentException">The zip file could not be opened.</exception>
    /// <exception cref="IOException">The file could not be written.</exception>
    /// <exception cref="InvalidOperationException">The data from the API is in an unrecognized format.</exception>
    static async Task<Stream> Reader(string? apWorld, CancellationToken token)
    {
        if (apWorld is not null)
            return File.Open(apWorld, FileMode.Open, FileAccess.Read);

        const string
            Api = "https://api.github.com/repos/ManualForArchipelago/Manual/releases/latest",
            Error = $"GET request was successful, but the download link could not be resolved from: {Api}";

        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("User-Agent", ".NET Runtime Application");
        var json = await client.GetFromJsonAsync<JsonObject>(Api, token);
        var url = json?["assets"]?[0]?["browser_download_url"]?.GetValue<string>();
        return await client.GetStreamAsync(url ?? throw new InvalidOperationException(Error), token);
    }

    /// <summary>Gets the number of times the item is guaranteed to be non-local.</summary>
    /// <param name="item">The item to check.</param>
    /// <returns>The number of times it will be non-local.</returns>
    [Pure]
    int CountExported(Item item)
    {
        var world = World;

        return item.Priority.Has(Priority.Local)
            ? 0
            : item.Count -
            item.LocalEarly.Max(
                world?.AllLocations.Count(
                    x => x.ItemAllowList is [var it] && it.Name == item.Name ||
                        x.CategoryAllowList is [var category] &&
                        (IList<Item>)[..world.AllItemsWith(category)] is [var i] &&
                        i.Name == item.Name
                ) ??
                0
            );
    }

    /// <summary>Gets the minimum <see cref="Item.Count"/> from categories.</summary>
    /// <param name="categories">The categories to check.</param>
    /// <returns>The minimum <see cref="Item.Count"/>.</returns>
    [Pure]
    int Min(ArchipelagoListBuilder<Category> categories) =>
        World?.AllItemsWithAny(categories).Aggregate(int.MaxValue, (a, n) => a.Min(n.Count)) ?? 0;

    /// <summary>Gets the minimum number of items the block will take.</summary>
    /// <param name="block">The block to check.</param>
    /// <returns>The minimum number of items that <paramref name="block"/> will take.</returns>
    [Pure]
    int Min(StartingItemBlock block) =>
        block.Amount ??
        (World is null ? 0 :
            block.Set is var set &&
            set is { IsCategoryEmpty: true, IsItemEmpty: true } ?
                World.AllItems.Aggregate(0, (a, n) => a.Min(n.Count)) :
                set is { IsCategoryEmpty: true } ? Min(set.ItemBuilder) :
                    set is { IsItemEmpty: true } ? Min(set.CategoryBuilder) :
                        Min(set.CategoryBuilder).Min(Min(set.ItemBuilder)));

    [Pure]
    string FullName() => $"Manual_{Name}_{(Creator.IsEmpty ? Environment.UserName : Creator)}";

    [Pure]
    string? GameName() =>
        Name.IsEmpty ? new StackFrame(2).GetMethod()?.ReflectedType?.Assembly.GetName().Name : ToString();
}
