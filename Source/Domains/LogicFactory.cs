// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;
#pragma warning disable MA0016
/// <summary>Provides methods for creating <see cref="Logic"/> instances.</summary>
public static partial class LogicFactory
{
    /// <summary>Creates the <see cref="Logic"/> that requires all of the provided items.</summary>
    /// <param name="items">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? And(this IEnumerable<Item> items) => items.Aggregate((Logic?)null, (a, n) => a & n);

    /// <summary>Creates the <see cref="Logic"/> that requires all provided logic.</summary>
    /// <param name="logic">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? And(this IEnumerable<Logic?> logic) => logic.Aggregate((Logic?)null, (a, n) => a & n);

    /// <summary>Creates the <see cref="Logic"/> that requires any of the provided items.</summary>
    /// <param name="items">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? Or(this IEnumerable<Item> items) => items.Aggregate((Logic?)null, (a, n) => a | n);

    /// <summary>Creates the <see cref="Logic"/> that requires any provided logic.</summary>
    /// <param name="logic">The items to wrap in <see cref="Logic"/>.</param>
    /// <returns>The created <see cref="Logic"/>.</returns>
    public static Logic? Or(this IEnumerable<Logic?> logic) => logic.Aggregate((Logic?)null, (a, n) => a | n);

    /// <summary>Syncs the <see cref="Dictionary{TKey, TValue}"/> and itself to contain the same values.</summary>
    /// <param name="builder">The passages to check.</param>
    /// <param name="categories">The categories to synchronize.</param>
    /// <param name="items">The items to synchronize.</param>
    /// <param name="regions">The regions to synchronize.</param>
    /// <param name="fallback">The fallback priority for when one isn't specified.</param>
    /// <param name="strict">Whether to throw an exception when any parameter does not contain the key.</param>
    /// <exception cref="KeyNotFoundException">
    /// This list contains an element whose name does not exist in one of the dictionaries,
    /// and the parameter <paramref name="strict"/> is <see langword="true"/>.
    /// </exception>
    public static void Sync(
        this in ArchipelagoListBuilder<Passage> builder,
        Dictionary<string, Category> categories,
        Dictionary<string, Item> items,
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
            ArchipelagoListBuilder<Region>.Sync(regions, ref region, fallback, strict);
            logic?.ThrowIfUnreferenced(categories, items, strict);
        }
    }
}
