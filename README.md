[<img src="Images/icon.svg" width=25% height=25%>](https://raw.githubusercontent.com/Emik03/Emik.Manual/refs/heads/main/Images/icon.svg)

# Emik.Manual

[![NuGet package](https://img.shields.io/nuget/v/Emik.Manual.svg?color=50fa7b&logo=NuGet&style=for-the-badge)](https://www.nuget.org/packages/Emik.Manual)
[![License](https://img.shields.io/github/license/Emik03/Emik.Manual.svg?color=6272a4&style=for-the-badge)](https://github.com/Emik03/Emik.Manual/blob/main/LICENSE)

.NET 9.0+ library to programmatically create a [manual `.apworld`](https://github.com/ManualForArchipelago) for use in [Archipelago](https://github.com/ArchipelagoMW/Archipelago). 

This project has a dependency to [Emik.Morsels](https://github.com/Emik03/Emik.Morsels), if you are building this project, refer to its [README](https://github.com/Emik03/Emik.Morsels/blob/main/README.md) first.

---

- [Examples](#examples)
- [Contribute](#contribute)
- [License](#license)

---

## Examples

This implementation is for the video game [Space Invaders: Infinity Gene](https://en.wikipedia.org/wiki/Space_Invaders_Infinity_Gene). To run this yourself, it requires [Emik.Morsels](https://github.com/Emik03/Emik.Morsels), OR [just this file](https://raw.githubusercontent.com/Emik03/Emik.Morsels/refs/heads/main/Snippets/REPL.csx) if you're simply running it in a REPL. For additional examples, take a look at [Emik.Manual.Examples](https://github.com/Emik03/Emik.Manual.Examples), each project ranging in complexity.

```csharp
using Emik.Manual;
using Emik.Manual.Domains;

static IAsyncEnumerable<SplitMemory<char, char, MatchOne>> Read([Match("^[^<>:\"/\\\\|?*]+$")] string path)
{
    var found = Path.Join(Environment.CurrentDirectory)
       .FindPathToNull(Path.GetDirectoryName)
       .Select(x => Path.Join(x, path))
       .FirstOrDefault(File.Exists);

    return File.ReadLinesAsync(found ?? throw new FileNotFoundException(null, path)).Select(x => x.SplitOn(','));
}

World world = new();

ImmutableArray<(string Name, int Count)> arms =
    [("RAPID SHOT", 1), ("SEARCH LASER", 1), ("WAVE", 1), ("LOCK-ON", 4), ("GRAVITY", 2), ("ROUND", 1), ("CLASSIC", 4)];

var armCategory = world.Category("ARMS");
var starterArmCategory = world.Category("STARTER ARMS", true);

foreach (var (name, count) in arms)
    world.Item(
        $"PROGRESSIVE {name} ({count})",
        count is 1 ? Priority.Useful : Priority.Progression,
        [world.Category(name), armCategory, ..count is 1 ? (ReadOnlySpan<Category>)[starterArmCategory] : []],
        count
    );

var settings = world.Category("SETTINGS");
world.Item("TOGGLE COMMENT", Priority.Filler, settings, 7);
var autoShot = world.Item("AUTO SHOT", Priority.Progression, settings);
var stocks = world.Item("PROGRESSIVE STOCKS (+1 LIFE)", null, settings, 8);
var strongestArms = world.Item("LOCK-ON").All | world.Item("CLASSIC").All;
var strongArms = world.Item("GRAVITY").All | strongestArms;

await foreach (var (location, (categoryName, (hintEntrance, (meta, _)))) in Read("InfinityGeneChecks.csv"))
    world.Location(
        location,
        categoryName.Span switch
        {
            "LEVEL 0" => stocks[0],
            "LEVEL 1" => stocks[0],
            "LEVEL 2" => stocks[0],
            "LEVEL 3" => strongArms & stocks[2],
            "EXTRA LEVEL 1" => strongArms & stocks[4],
            "EXTRA LEVEL 2" => strongestArms & stocks[6],
            "EXTRA LEVEL 3" => strongestArms & stocks[8] & autoShot,
            _ => throw new UnreachableException(categoryName.ToString()),
        } &
        (world.AllLocations.TryGetValue(meta, out var inheritedLogic) ? inheritedLogic.Logic : null) &
        world.Item(categoryName, Priority.ProgressionUseful, world.Category("LEVELS")),
        [world.Category(categoryName), world.Category(location)],
        null,
        meta.Span is "VICTORY" ? LocationOptions.Victory : LocationOptions.None,
        hintEntrance: hintEntrance
    );

await world.Game("InfinityGene", "Emik", "EVOLUTION", [world.AllItems["LEVEL 0"], new(starterArmCategory, 1)])
   .DisplayExported(Console.WriteLine)
   .ZipAsync(Path.GetTempPath(), listChecks: true);
```

## Contribute

Issues and pull requests are welcome to help this repository be the best it can be.

## License

This repository falls under the [MPL-2 license](https://www.mozilla.org/en-US/MPL/2.0/).
