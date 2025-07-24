// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;
#pragma warning disable MA0016
using static Logic.Kind;

/// <summary>Provides methods for creating <see cref="Logic"/> instances.</summary>
public static partial class LogicFactory
{
    /// <summary>Syncs the <see cref="Dictionary{TKey, TValue}"/> and itself to contain the same values.</summary>
    /// <param name="builder">The passages to check.</param>
    /// <param name="categories">The categories to synchronize.</param>
    /// <param name="items">The items to synchronize.</param>
    /// <param name="locations">The locations to synchronize.</param>
    /// <param name="regions">The regions to synchronize.</param>
    /// <param name="fallback">The fallback priority for when one isn't specified.</param>
    /// <param name="strict">Whether to throw an exception when any parameter does not contain the key.</param>
    /// <exception cref="KeyNotFoundException">
    /// This list contains an element whose name does not exist in one of the dictionaries,
    /// and the parameter <paramref name="strict"/> is <see langword="true"/>.
    /// </exception>
    public static void Sync(
        this in ArchipelagoBuilder<Passage> builder,
        Dictionary<string, Category> categories,
        Dictionary<string, Item> items,
        Dictionary<string, Location> locations,
        Dictionary<string, Region> regions,
        Priority fallback = Priority.None,
        bool strict = true
    )
    {
        if (builder.IsDefault)
            return;

        for (int i = 0, count = builder.Count; i < count; i++)
        {
            var (region, logic) = builder[i];
            ArchipelagoBuilder<Region>.Sync(regions, ref region, fallback, strict);
            logic?.ThrowIfUnset(categories, items, locations, regions, strict);
        }
    }

    /// <summary>Determines if the instance passed in doesn't contribute to any meaningful logic.</summary>
    /// <param name="logic">The instance to check.</param>
    /// <returns>Whether this logic will be inlined.</returns>
    public static bool IsRedundant([NotNullWhen(false)] this Logic? logic) =>
        logic is null or { Count: 0, Type: ItemCount or ItemPercent or CategoryCount or CategoryPercent or ItemValue };

    /// <summary>Creates the <see cref="Logic"/> that requires all of the provided categories.</summary>
    /// <param name="categories">The categories to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? And(this IEnumerable<Category> categories) =>
        categories.Aggregate((Logic?)null, (a, n) => a & n);

    /// <summary>Creates the <see cref="Logic"/> that requires all of the provided items.</summary>
    /// <param name="items">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? And(this IEnumerable<Item> items) => items.Aggregate((Logic?)null, (a, n) => a & n);

    /// <summary>Creates the <see cref="Logic"/> that requires all of the provided locations.</summary>
    /// <param name="locations">The locations to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? And(this IEnumerable<Location> locations) =>
        locations.Aggregate((Logic?)null, (a, n) => a & n);

    /// <summary>Creates the <see cref="Logic"/> that requires all provided logic.</summary>
    /// <param name="logic">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? And(this IEnumerable<Logic?> logic) => logic.Aggregate((Logic?)null, (a, n) => a & n);

    /// <summary>
    /// Creates the <see cref="Logic"/> object of <see cref="Logic.Builtin.OptOne"/> or
    /// <see cref="Logic.Builtin.OptAll"/>, depending on the <see cref="Logic"/> parameter.
    /// </summary>
    /// <param name="logic">The logic to wrap.</param>
    /// <returns>The new <see cref="Logic"/> object containing the parameter <paramref name="logic"/>.</returns>
    /// <seealso cref="Logic.Builtin.OptOne"/>
    /// <seealso cref="Logic.Builtin.OptAll"/>
    [Pure]
    [return: NotNullIfNotNull(nameof(logic))]
    public static Logic? Opt(this Logic? logic) => logic is null ? null : new(logic);

    /// <summary>Creates the <see cref="Logic"/> that requires any of the provided categories.</summary>
    /// <param name="categories">The categories to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? Or(this IEnumerable<Category> categories) =>
        categories.Aggregate((Logic?)null, (a, n) => a | n);

    /// <summary>Creates the <see cref="Logic"/> that requires any of the provided items.</summary>
    /// <param name="items">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? Or(this IEnumerable<Item> items) => items.Aggregate((Logic?)null, (a, n) => a | n);

    /// <summary>Creates the <see cref="Logic"/> that requires any of the provided locations.</summary>
    /// <param name="locations">The locations to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? Or(this IEnumerable<Location> locations) => locations.Aggregate((Logic?)null, (a, n) => a | n);

    /// <summary>Creates the <see cref="Logic"/> that requires any provided logic.</summary>
    /// <param name="logic">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? Or(this IEnumerable<Logic?> logic) => logic.Aggregate((Logic?)null, (a, n) => a | n);

    /// <summary>Returns itself, discarding the parameter. Prints both if compiled with a specific constant.</summary>
    /// <param name="left">The logic to return.</param>
    /// <param name="right">The logic to discard.</param>
    /// <param name="line">The caller line number to discard.</param>
    /// <returns>Itself.</returns>
    [return: NotNullIfNotNull(nameof(left))]
    internal static Logic? Check(
        this Logic? left,
        [UsedImplicitly] Logic? right,
        [CallerLineNumber, UsedImplicitly] int line = 0
    )
    {
#if LOGIC_TRIM_CONSOLE_WRITE
        Console.WriteLine(
            $"""
             Found optimization: {line}
             Preserving: {left}
             Discarding: {right}

             """
        );
#endif
        left?.IsOptimized = true;
        return left;
    }
}
